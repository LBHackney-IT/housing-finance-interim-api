using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using GD = Google.Apis.Drive.v3.Data;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Moq;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class GenerateReportUseCaseTests
    {
        private readonly Mock<IBatchReportGateway> _mockBatchReportGateway;
        private readonly Mock<IReportGateway> _mockReportGateway;
        private readonly Mock<IGoogleFileSettingGateway> _mockGoogleFileSettingGateway;
        private readonly Mock<IGoogleClientService> _mockGoogleClientService;
        private readonly int _waitDuration = 30;
        private readonly IGenerateReportUseCase _classUnderTest;

        public GenerateReportUseCaseTests()
        {
            _mockBatchReportGateway = new Mock<IBatchReportGateway>();
            _mockReportGateway = new Mock<IReportGateway>();
            _mockGoogleFileSettingGateway = new Mock<IGoogleFileSettingGateway>();
            _mockGoogleClientService = new Mock<IGoogleClientService>();

            Environment.SetEnvironmentVariable("WAIT_DURATION", _waitDuration.ToString());

            _classUnderTest = new GenerateReportUseCase(
                    _mockBatchReportGateway.Object,
                    _mockReportGateway.Object,
                    _mockGoogleFileSettingGateway.Object,
                    _mockGoogleClientService.Object
                );            
        }

        #region Shared
        [Fact]
        public async Task GenerateReportUCChecksWhetherAnyUnprocessedReportRequestsExist()
        {
            // arrange
            var unprocessedReports = new List<BatchReportDomain>();

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockBatchReportGateway.Verify(g => g.ListPendingAsync(), Times.Once);
        }

        [Fact]
        public async Task GenerateReportUCTerminatesExecutionAndReturnsApproapriateResponseWhenThereAreNoReportRequestsToProcess()
        {
            // arrange
            var nextStepTime = DateTime.Now.AddSeconds(_waitDuration);
            var unprocessedReports = new List<BatchReportDomain>();

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            // act
            var stepResponse = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleFileSettingGateway.Verify(g => g.GetSettingsByLabel(It.IsAny<string>()), Times.Never);

            stepResponse.Should().NotBeNull();
            stepResponse.Continue.Should().BeFalse();
            stepResponse.NextStepTime.Should().BeCloseTo(nextStepTime, precision: 1000);
        }

        [Fact]
        public async Task GenerateReportUCTerminatesExecutionAndReturnsApproapriateResponseWhenTheRequestedReportNameFoundIsUnknown()
        {
            // arrange
            var nextStepTime = DateTime.Now.AddSeconds(_waitDuration);

            var unknownTypeUnprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, "Pepsi > CocaCola")
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unknownTypeUnprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            // act
            var stepResponse = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleFileSettingGateway.Verify(g => g.GetSettingsByLabel(It.IsAny<string>()), Times.Never);

            stepResponse.Should().NotBeNull();
            stepResponse.Continue.Should().BeTrue();
            stepResponse.NextStepTime.Should().BeCloseTo(nextStepTime, precision: 1000);
        }

        [Fact]
        public async Task GenerateReportUCStartsGeneratingTheEarliestRequestedReportInTheQueue()
        {
            // arrange
            var expectedearliestReportLabel = "ReportItemisedTransactions";

            var earliestRequestedUnprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, expectedearliestReportLabel)
                .With(r => r.StartTime, DateTime.Now.AddMinutes(-20))
                .CreateCustom();

            var lastRequestedUnprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, "ReportCashSuspense")
                .With(r => r.StartTime, DateTime.Now)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { lastRequestedUnprocessedReport, earliestRequestedUnprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var accountBalanceFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, expectedearliestReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { accountBalanceFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var irrelevantFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(irrelevantFile);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleFileSettingGateway.Verify(g => g.GetSettingsByLabel(It.Is<string>(l => l == expectedearliestReportLabel)), Times.Once);
        }

        [Fact]
        public async Task GenerateReportUCThrowsAnExceptionWheneverOneIsRaisedWithinIt()
        {
            // arrange
            var message = "The premise of the argument is incorrect.";
            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ThrowsAsync(new ArgumentException(message));

            // act
            Func<Task> generateReportCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            await generateReportCall.Should().ThrowAsync<ArgumentException>().WithMessage(message);
        }
        #endregion

        #region Account Balance
        [Fact]
        public async Task GenerateReportUCSearchesForAnApproapriateGoogleFileSettingWhenRequestedReportIsAnAccountBalance()
        {
            // arrange
            var requestedReportLabel = "ReportAccountBalanceByDate";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var accountBalanceFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { accountBalanceFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var irrelevantFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(irrelevantFile);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleFileSettingGateway.Verify(g => g.GetSettingsByLabel(It.Is<string>(l => l == requestedReportLabel)), Times.Once);
        }

        [Fact]
        public async Task GenerateReportUCMarksReportRequestAsFailureAndTerminatesExecutionWhenAccountBalanceFolderIdIsNotFound()
        {
            // arrange
            var requestedReportLabel = "ReportAccountBalanceByDate";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>();

            _mockGoogleFileSettingGateway.Setup(g => g.GetSettingsByLabel(It.Is<string>(l => l == requestedReportLabel))).ReturnsAsync(googleFileSettingsFound);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockBatchReportGateway.Verify(
                g => g.SetStatusAsync(
                    It.Is<int>(id => id == unprocessedReport.Id),
                    It.Is<string>(l => l == "Output folder not found"),
                    It.Is<bool>(s => s == false)
                ),
                Times.Once
            );

            _mockReportGateway.Verify(
                g => g.GetReportAccountBalanceAsync(
                    It.IsAny<DateTime>(),
                    It.IsAny<string>()
                ),
                Times.Never
            );
        }

        [Fact]
        public async Task GenerateReportUCCallsTheReportGWGetReportAccountBalanceWithDateTimeAndRentGroupFromTheReportRequestAlsoTheCreatedFileNameIsHasTrimmedRentGroup()
        {
            // arrange
            var requestedRentGroup = "HRA";
            var untrimmedRentGroup = $"  {requestedRentGroup} ";
            var requestedReportLabel = "ReportAccountBalanceByDate";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .With(r => r.RentGroup, untrimmedRentGroup)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var accountBalanceFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { accountBalanceFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var irrelevantFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(irrelevantFile);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockReportGateway.Verify(
                g => g.GetReportAccountBalanceAsync(
                    It.Is<DateTime>(d => d.Equals(unprocessedReport.ReportDate.Value)),
                    It.Is<string>(r => r == untrimmedRentGroup) // Odd, but that's the current behaviour.
                ),
                Times.Once
            );

            _mockGoogleClientService.Verify(
                g => g.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.Is<string>(fn =>
                        !fn.Contains(untrimmedRentGroup) &&
                        fn.Contains(requestedRentGroup)
                    ),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GenerateReportUCCallsTheReportGWGetReportAccountBalanceWithRentGroupValueSetAsNullButTheUploadedFileNameContainsTheValueALLWhenRentGroupIsNotProvided()
        {
            // arrange
            var requestedReportLabel = "ReportAccountBalanceByDate";

            string requestedRentGroup = null;
            var allRentGroups = "ALL";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .With(r => r.RentGroup, requestedRentGroup)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var accountBalanceFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { accountBalanceFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var irrelevantFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(irrelevantFile);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockReportGateway.Verify(
                g => g.GetReportAccountBalanceAsync(
                    It.IsAny<DateTime>(),
                    It.Is<string>(r => r == requestedRentGroup)
                ),
                Times.Once
            );

            _mockGoogleClientService.Verify(
                g => g.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.Is<string>(fn => fn.Contains(allRentGroups)),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GenerateReportUCUploadsTheAccountBalanceDataRetrievedFromDabataseAsCSVIntoExpectedFolderUnderANameSpecifyingAReportType()
        {
            // arrange
            var requestedReportLabel = "ReportAccountBalanceByDate";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var accountBalanceFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { accountBalanceFolder };

            _mockGoogleFileSettingGateway.Setup(g => g.GetSettingsByLabel(It.Is<string>(l => l == requestedReportLabel))).ReturnsAsync(googleFileSettingsFound);

            var spreadSheetData = new List<string[]>() {
                new string [] { "header 1", "header 2", "header 3", "header 4" },
                new string [] { "0008425", "HRA", "520.36", "2020-08-08" }
            };

            _mockReportGateway
                .Setup(g => g.GetReportAccountBalanceAsync(
                    It.IsAny<DateTime>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(spreadSheetData);

            var irrelevantFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(irrelevantFile);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleClientService.Verify(
                g => g.UploadCsvFile(
                    It.Is<List<string[]>>(t => ReferenceEquals(t, spreadSheetData)),
                    It.Is<string>(fn => fn.Contains("Account_Balance")),
                    It.Is<string>(id => id == accountBalanceFolder.GoogleIdentifier)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GenerateReportUCRetrievesTheUploadedAccountBalanceCSVFileIdAndUpdatesTheReportRequestByAttachingAFileLinkToItAndSettingItSuccessThenTheUCReturnsCorrectStepResponse()
        {
            // arrange
            var requestedReportLabel = "ReportAccountBalanceByDate";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var accountBalanceFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { accountBalanceFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var spreadSheetData = new List<string[]>();

            _mockReportGateway
                .Setup(g => g.GetReportAccountBalanceAsync(
                    It.IsAny<DateTime>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(spreadSheetData);

            _mockGoogleClientService
                .Setup(g => g.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(true);

            var uploadedCSVFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(uploadedCSVFile);

            // act
            var stepResponse = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleClientService.Verify(
                g => g.GetFileByNameInDriveAsync(
                    It.Is<string>(fid => fid == accountBalanceFolder.GoogleIdentifier),
                    It.Is<string>(fn =>
                        fn.Contains("Account_Balance") &&
                        fn.Contains(unprocessedReport.RentGroup) &&
                        fn.Contains(unprocessedReport.Id.ToString())
                )),
                Times.AtLeastOnce
            );

            _mockBatchReportGateway.Verify(
                g => g.SetStatusAsync(
                    It.Is<int>(id => id == unprocessedReport.Id),
                    It.Is<string>(link => link == $"https://drive.google.com/file/d/{uploadedCSVFile.Id}"),
                    It.Is<bool>(isSuccess => isSuccess == true)
                ),
                Times.Once
            );

            var nextStepTime = DateTime.Now.AddSeconds(_waitDuration);

            stepResponse.Should().NotBeNull();
            stepResponse.Continue.Should().BeTrue();
            stepResponse.NextStepTime.Should().BeCloseTo(nextStepTime, precision: 1000);
        }
        #endregion

        #region Charges
        [Fact]
        public async Task GenerateReportUCSearchesForAnApproapriateGoogleFileSettingWhenRequestedReportIsCharges()
        {
            // arrange
            var requestedReportLabel = "ReportCharges";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var chargesFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { chargesFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var irrelevantFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(irrelevantFile);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleFileSettingGateway.Verify(g => g.GetSettingsByLabel(It.Is<string>(l => l == requestedReportLabel)), Times.Once);
        }

        [Fact]
        public async Task GenerateReportUCMarksReportRequestAsFailureAndTerminatesExecutionWhenChargesFolderIdIsNotFound()
        {
            // arrange
            var requestedReportLabel = "ReportCharges";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>();

            _mockGoogleFileSettingGateway.Setup(g => g.GetSettingsByLabel(It.Is<string>(l => l == requestedReportLabel))).ReturnsAsync(googleFileSettingsFound);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockBatchReportGateway.Verify(
                g => g.SetStatusAsync(
                    It.Is<int>(id => id == unprocessedReport.Id),
                    It.Is<string>(l => l == "Output folder not found"),
                    It.Is<bool>(s => s == false)
                ),
                Times.Once
            );

            _mockReportGateway.Verify(
                g => g.GetChargesByYearAndRentGroupAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ),
                Times.Never
            );

            _mockReportGateway.Verify(
                g => g.GetChargesByGroupTypeAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ),
                Times.Never
            );

            _mockReportGateway.Verify(
                g => g.GetChargesByYearAsync(
                    It.IsAny<int>()
                ),
                Times.Never
            );
        }

        [Fact]
        public async Task GenerateReportUCCallsTheReportGWGetChargesByYearAndRentGroupWithApproapriateParametersAndFileNameShouldContainThoseParamertersWhenTheRentGroupIsNonEmpty()
        {
            // arrange
            var requestedReportLabel = "ReportCharges";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var chargesFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { chargesFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var irrelevantFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(irrelevantFile);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockReportGateway.Verify(
                g => g.GetChargesByYearAndRentGroupAsync(
                    It.Is<int>(d => d.Equals(unprocessedReport.ReportYear.Value)),
                    It.Is<string>(r => r == unprocessedReport.RentGroup)
                ),
                Times.Once
            );

            _mockReportGateway.Verify(
                g => g.GetChargesByGroupTypeAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ),
                Times.Never
            );

            _mockReportGateway.Verify(
                g => g.GetChargesByYearAsync(
                    It.IsAny<int>()
                ),
                Times.Never
            );

            _mockGoogleClientService.Verify(
                g => g.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.Is<string>(fn =>
                        fn.Contains(unprocessedReport.ReportYear.Value.ToString()) &&
                        fn.Contains(unprocessedReport.RentGroup) &&
                        !fn.Contains(unprocessedReport.Group)
                    ),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GenerateReportUCCallsTheReportGWGetChargesByGroupTypeWithApproapriateParametersAndFileNameShouldContainThoseParamertersWhenTheGroupIsNonEmptyButTheRentGroupIsEmpty()
        {
            // arrange
            var requestedReportLabel = "ReportCharges";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .With(r => r.RentGroup, null as string)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var chargesFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { chargesFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var irrelevantFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(irrelevantFile);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockReportGateway.Verify(
                g => g.GetChargesByYearAndRentGroupAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ),
                Times.Never
            );

            _mockReportGateway.Verify(
                g => g.GetChargesByGroupTypeAsync(
                    It.Is<int>(d => d.Equals(unprocessedReport.ReportYear.Value)),
                    It.Is<string>(r => r == unprocessedReport.Group)
                ),
                Times.Once
            );

            _mockReportGateway.Verify(
                g => g.GetChargesByYearAsync(
                    It.IsAny<int>()
                ),
                Times.Never
            );

            _mockGoogleClientService.Verify(
                g => g.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.Is<string>(fn =>
                        fn.Contains(unprocessedReport.ReportYear.Value.ToString()) &&
                        fn.Contains(unprocessedReport.Group)
                    ),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GenerateReportUCCallsTheReportGWGetChargesByYearWithApproapriateParametersAndFileNameShouldContainThoseParamertersWhenBothTheGroupAndTheRentGroupAreEmpty()
        {
            // arrange
            var requestedReportLabel = "ReportCharges";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .With(r => r.RentGroup, null as string)
                .With(r => r.Group, null as string)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var chargesFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { chargesFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var irrelevantFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(irrelevantFile);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockReportGateway.Verify(
                g => g.GetChargesByYearAndRentGroupAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ),
                Times.Never
            );

            _mockReportGateway.Verify(
                g => g.GetChargesByGroupTypeAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ),
                Times.Never
            );

            _mockReportGateway.Verify(
                g => g.GetChargesByYearAsync(
                    It.Is<int>(d => d.Equals(unprocessedReport.ReportYear.Value))
                ),
                Times.Once
            );

            _mockGoogleClientService.Verify(
                g => g.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.Is<string>(fn =>
                        fn.Contains(unprocessedReport.ReportYear.Value.ToString())
                    ),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GenerateReportUCUploadsTheChargesByYearAndRentGroupDataRetrievedFromDabataseAsCSVIntoExpectedFolderUnderANameSpecifyingAReportType()
        {
            // arrange
            var requestedReportLabel = "ReportCharges";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var chargesFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { chargesFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var spreadSheetData = new List<string[]>() {
                new string [] { "header 1", "header 2", "header 3", "header 4" },
                new string [] { "0001234", "TRA", "130.36", "2025-01-06" }
            };

            _mockReportGateway
                .Setup(g => g.GetChargesByYearAndRentGroupAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(spreadSheetData);

            var irrelevantFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(irrelevantFile);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleClientService.Verify(
                g => g.UploadCsvFile(
                    It.Is<List<string[]>>(t => ReferenceEquals(t, spreadSheetData)),
                    It.Is<string>(fn => fn.Contains("Charges")),
                    It.Is<string>(id => id == chargesFolder.GoogleIdentifier)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GenerateReportUCUploadsTheChargesByGroupTypeDataRetrievedFromDabataseAsCSVIntoExpectedFolderUnderANameSpecifyingAReportType()
        {
            // arrange
            var requestedReportLabel = "ReportCharges";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .With(r => r.RentGroup, null as string)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var chargesFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { chargesFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var spreadSheetData = new List<string[]>() {
                new string [] { "header 1", "header 2", "header 3", "header 4" },
                new string [] { "0022455", "LSC", "7.88", "2023-04-22" }
            };

            _mockReportGateway
                .Setup(g => g.GetChargesByGroupTypeAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(spreadSheetData);

            var irrelevantFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(irrelevantFile);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleClientService.Verify(
                g => g.UploadCsvFile(
                    It.Is<List<string[]>>(t => ReferenceEquals(t, spreadSheetData)),
                    It.Is<string>(fn => fn.Contains("Charges")),
                    It.Is<string>(id => id == chargesFolder.GoogleIdentifier)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GenerateReportUCUploadsTheChargesByYearDataRetrievedFromDabataseAsCSVIntoExpectedFolderUnderANameSpecifyingAReportType()
        {
            // arrange
            var requestedReportLabel = "ReportCharges";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .With(r => r.RentGroup, null as string)
                .With(r => r.Group, null as string)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var chargesFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { chargesFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var spreadSheetData = new List<string[]>() {
                new string [] { "header 1", "header 2", "header 3", "header 4" },
                new string [] { "0004567", "LMW", "70.11", "2015-02-14" }
            };

            _mockReportGateway
                .Setup(g => g.GetChargesByYearAsync(
                    It.IsAny<int>()
                ))
                .ReturnsAsync(spreadSheetData);

            var irrelevantFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(irrelevantFile);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleClientService.Verify(
                g => g.UploadCsvFile(
                    It.Is<List<string[]>>(t => ReferenceEquals(t, spreadSheetData)),
                    It.Is<string>(fn => fn.Contains("Charges")),
                    It.Is<string>(id => id == chargesFolder.GoogleIdentifier)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GenerateReportUCRetrievesTheUploadedChargesByYearAndRentGroupCSVFileIdAndUpdatesTheReportRequestByAttachingAFileLinkToItAndSettingItSuccessThenTheUCReturnsCorrectStepResponse()
        {
            // arrange
            var requestedReportLabel = "ReportCharges";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var chargesFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { chargesFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var spreadSheetData = new List<string[]>();

            _mockReportGateway
                .Setup(g => g.GetChargesByYearAndRentGroupAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(spreadSheetData);

            _mockGoogleClientService
                .Setup(g => g.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(true);

            var uploadedCSVFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(uploadedCSVFile);

            // act
            var stepResponse = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleClientService.Verify(
                g => g.GetFileByNameInDriveAsync(
                    It.Is<string>(fid => fid == chargesFolder.GoogleIdentifier),
                    It.Is<string>(fn =>
                        fn.Contains("Charges") &&
                        fn.Contains(unprocessedReport.ReportYear.Value.ToString()) &&
                        fn.Contains(unprocessedReport.RentGroup.ToString()) &&
                        fn.Contains(unprocessedReport.Id.ToString())
                )),
                Times.AtLeastOnce
            );

            _mockBatchReportGateway.Verify(
                g => g.SetStatusAsync(
                    It.Is<int>(id => id == unprocessedReport.Id),
                    It.Is<string>(link => link == $"https://drive.google.com/file/d/{uploadedCSVFile.Id}"),
                    It.Is<bool>(isSuccess => isSuccess == true)
                ),
                Times.Once
            );

            stepResponse.Should().NotBeNull();
            stepResponse.Continue.Should().BeTrue();
            stepResponse.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(_waitDuration), 1000);
        }

        [Fact]
        public async Task GenerateReportUCRetrievesTheUploadedChargesByGroupTypeCSVFileIdAndUpdatesTheReportRequestByAttachingAFileLinkToItAndSettingItSuccessThenTheUCReturnsCorrectStepResponse()
        {
            // arrange
            var requestedReportLabel = "ReportCharges";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .With(r => r.RentGroup, null as string)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var chargesFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { chargesFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var spreadSheetData = new List<string[]>();

            _mockReportGateway
                .Setup(g => g.GetChargesByGroupTypeAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(spreadSheetData);

            _mockGoogleClientService
                .Setup(g => g.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(true);

            var uploadedCSVFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(uploadedCSVFile);

            // act
            var stepResponse = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleClientService.Verify(
                g => g.GetFileByNameInDriveAsync(
                    It.Is<string>(fid => fid == chargesFolder.GoogleIdentifier),
                    It.Is<string>(fn =>
                        fn.Contains("Charges") &&
                        fn.Contains(unprocessedReport.ReportYear.Value.ToString()) &&
                        fn.Contains(unprocessedReport.Group.ToString()) &&
                        fn.Contains(unprocessedReport.Id.ToString())
                )),
                Times.AtLeastOnce
            );

            _mockBatchReportGateway.Verify(
                g => g.SetStatusAsync(
                    It.Is<int>(id => id == unprocessedReport.Id),
                    It.Is<string>(link => link == $"https://drive.google.com/file/d/{uploadedCSVFile.Id}"),
                    It.Is<bool>(isSuccess => isSuccess == true)
                ),
                Times.Once
            );

            stepResponse.Should().NotBeNull();
            stepResponse.Continue.Should().BeTrue();
            stepResponse.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(_waitDuration), 1000);
        }

        [Fact]
        public async Task GenerateReportUCRetrievesTheUploadedChargesByYearCSVFileIdAndUpdatesTheReportRequestByAttachingAFileLinkToItAndSettingItSuccessThenTheUCReturnsCorrectStepResponse()
        {
            // arrange
            var requestedReportLabel = "ReportCharges";

            var unprocessedReport = RandomGen
                .Build<BatchReportDomain>()
                .With(r => r.ReportName, requestedReportLabel)
                .With(r => r.RentGroup, null as string)
                .With(r => r.Group, null as string)
                .CreateCustom();

            var unprocessedReports = new List<BatchReportDomain>() { unprocessedReport };

            _mockBatchReportGateway.Setup(g => g.ListPendingAsync()).ReturnsAsync(unprocessedReports);

            var chargesFolder = RandomGen
                .Build<GoogleFileSettingDomain>()
                .With(f => f.Label, requestedReportLabel)
                .CreateCustom();

            var googleFileSettingsFound = new List<GoogleFileSettingDomain>() { chargesFolder };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(googleFileSettingsFound);

            var spreadSheetData = new List<string[]>();

            _mockReportGateway
                .Setup(g => g.GetChargesByYearAsync(
                    It.IsAny<int>()
                ))
                .ReturnsAsync(spreadSheetData);

            _mockGoogleClientService
                .Setup(g => g.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(true);

            var uploadedCSVFile = RandomGen.Create<GD.File>();

            _mockGoogleClientService
                .Setup(g => g.GetFileByNameInDriveAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ReturnsAsync(uploadedCSVFile);

            // act
            var stepResponse = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleClientService.Verify(
                g => g.GetFileByNameInDriveAsync(
                    It.Is<string>(fid => fid == chargesFolder.GoogleIdentifier),
                    It.Is<string>(fn =>
                        fn.Contains("Charges") &&
                        fn.Contains(unprocessedReport.ReportYear.Value.ToString()) &&
                        fn.Contains(unprocessedReport.Id.ToString())
                )),
                Times.AtLeastOnce
            );

            _mockBatchReportGateway.Verify(
                g => g.SetStatusAsync(
                    It.Is<int>(id => id == unprocessedReport.Id),
                    It.Is<string>(link => link == $"https://drive.google.com/file/d/{uploadedCSVFile.Id}"),
                    It.Is<bool>(isSuccess => isSuccess == true)
                ),
                Times.Once
            );

            stepResponse.Should().NotBeNull();
            stepResponse.Continue.Should().BeTrue();
            stepResponse.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(_waitDuration), 1000);
        }
        #endregion

    }
}