using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoFixture;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Domain;
using System.Linq;
using Google.Apis.Drive.v3.Data;
using FluentAssertions;
using SIO = System.IO;
using Google;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class MoveHousingBenefitFileUseCaseTests
    {
        private readonly Mock<IBatchLogGateway> _mockBatchLogGateway;
        private readonly Mock<IBatchLogErrorGateway> _mockBatchLogErrorGateway;
        private readonly Mock<IGoogleClientService> _mockGoogleClientService;
        private readonly Mock<IGoogleFileSettingGateway> _mockGoogleFileSettingGateway;
        private readonly IMoveHousingBenefitFileUseCase _classUnderTest;
        private readonly int _nextStepDelayDurationInSeconds;

        public MoveHousingBenefitFileUseCaseTests()
        {
            _mockBatchLogGateway = new Mock<IBatchLogGateway>();
            _mockBatchLogErrorGateway = new Mock<IBatchLogErrorGateway>();
            _mockGoogleClientService = new Mock<IGoogleClientService>();
            _mockGoogleFileSettingGateway = new Mock<IGoogleFileSettingGateway>();

            var waitDurationEnvVar = 30;
            Environment.SetEnvironmentVariable("WAIT_DURATION", waitDurationEnvVar.ToString());
            _nextStepDelayDurationInSeconds = waitDurationEnvVar;

            _classUnderTest = new MoveHousingBenefitFileUseCase(
                _mockBatchLogGateway.Object,
                _mockBatchLogErrorGateway.Object,
                _mockGoogleFileSettingGateway.Object,
                _mockGoogleClientService.Object
            );

            // Default mock responses that keep code on happy path for more readable test setups:
            var academyFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);
            var academyFolderFiles = RandomGen.GoogleDriveFiles(filesValidity: true, count: 1);
            var destinationFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);

            _mockBatchLogGateway.Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(RandomGen.BatchLogDomain());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.HousingBenefitFileLabel)))
                .ReturnsAsync(destinationFolders);

            // destination folder files
            _mockGoogleClientService.Setup(g => g.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<File>());

            // source folder files
            _mockGoogleClientService
                .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolders.First().GoogleIdentifier), It.IsAny<string>()))
                .ReturnsAsync(academyFolderFiles);
        }

        // UC calls the Batch Log GW with correct params
        [Fact]
        public async Task MoveHousingBenefitFileUCCallsBatchLogGWCreateMethodToRegisterTheProcessGettingTriggered()
        {
            // arrange
            var expectedProcessType = ConstantsGen.AcademyFileFolderLabel;
            var expectedProcessStatus = false;

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockBatchLogGateway.Verify(
                g => g.CreateAsync(
                    It.Is<string>(s => s == expectedProcessType),
                    It.Is<bool>(b => b == expectedProcessStatus)
                ),
                Times.Once
            );
        }

        // UC calls the File Settings GW with correct params
        [Fact]
        public async Task UCCallsGoogleFileSettingGWGetSettingsByLabelMethodWithAcademyLabel()
        {
            // arrange
            var expectedLabel = ConstantsGen.AcademyFileFolderLabel;

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleFileSettingGateway.Verify(
                g => g.GetSettingsByLabel(It.Is<string>(s => s == expectedLabel)),
                Times.Once
            );
        }

        // UC calls the File Settings GW with correct other params
        [Fact]
        public async Task UCCallsGoogleFileSettingGWGetSettingsByLabelMethodWithHousingBenefitLabel()
        {
            // arrange
            var expectedLabel = ConstantsGen.HousingBenefitFileLabel;

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleFileSettingGateway.Verify(
                g => g.GetSettingsByLabel(It.Is<string>(s => s == expectedLabel)),
                Times.Once
            );
        }

        // UC calls Get GDrive Files with each! ACADEMY file setting id Given by FS GW
        [Fact]
        public async Task UCCallsGoogleClientServiceGetFilesInDriveMethodWithEachAcademyFolderIdentifierReturnedByGoogleFileSettingGateway()
        {
            // arrange
            var academyFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>();

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFileSettings);

            // Return a couple files to avoid failure
            _mockGoogleClientService
                .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFileSettings.First().GoogleIdentifier), It.IsAny<string>()))
                .ReturnsAsync(RandomGen.GoogleDriveFiles(true));

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            academyFileSettings.ForEach((academyFileSetting) =>
                _mockGoogleClientService.Verify(
                    g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFileSetting.GoogleIdentifier), It.IsAny<string>()),
                    Times.Once
                )
            );
        }

        // UC calls google files GET for each destination folder
        [Fact]
        public async Task UCCallsGoogleClientServiceGetFilesInDriveMethodWithEachHousingBenefitDestinationFolderGIdReturnedByGoogleFileSettingGateway()
        {
            // arrange
            var destinationFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(1);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.HousingBenefitFileLabel)))
                .ReturnsAsync(destinationFileSettings);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            destinationFileSettings.ForEach(destinationFolder =>
                _mockGoogleClientService.Verify(
                    g => g.GetFilesInDriveAsync(It.Is<string>(s => s == destinationFolder.GoogleIdentifier), It.IsAny<string>()),
                    Times.Once
                )
            );
        }

        // It calls the CopyFileInDrive method only for the latest (created date) file that doesn't already exist at destination:
        [Fact]
        public async Task CopiesMostRecentAcademyFileThatDoesNotExistInDestFolder()
        {
            // arrange
            var academyNewFilesCount = 2;

            var referenceDate = new DateTime(2023, 04, 25);

            // Create 2 new files
            var academyFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);
            var academyNewFiles = RandomGen.GoogleDriveFiles(filesValidity: true, count: academyNewFilesCount);

            var newFiles = academyNewFiles as File[] ?? academyNewFiles.ToArray();
            // newFiles[0].Name = "rentpost_07042022_to_21042025";
            newFiles[0].CreatedTime = referenceDate - TimeSpan.FromDays(5);
            newFiles[1].CreatedTime = referenceDate - TimeSpan.FromDays(12);

            var expectedCopiedFileNewName = "HousingBenefitFile20230424.dat";

            var academyFolderFiles = newFiles;

            var destinationFolderFiles = new List<File>();

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders);

            var destinationFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.HousingBenefitFileLabel)))
                .ReturnsAsync(destinationFolders);

            var academyFolderGId = academyFolders.First().GoogleIdentifier;
            var destinationFolderGId = destinationFolders.First().GoogleIdentifier;

            _mockGoogleClientService
                    .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolderGId), It.IsAny<string>()))
                    .ReturnsAsync(academyFolderFiles);

            _mockGoogleClientService
                    .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == destinationFolderGId), It.IsAny<string>()))
                    .ReturnsAsync(destinationFolderFiles);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleClientService.Verify(
                g => g.CopyFileInDrive(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once
            );

            _mockGoogleClientService.Verify(
                service => service.CopyFileInDrive(
                    newFiles[0].Id,
                        destinationFolderGId,
                        expectedCopiedFileNewName
                    )
                );
        }

        [Fact]
        public async Task DoesNotCopyMostRecentFileIfNameExistsInDestinationFolder()
        {
            // arrange
            var referenceDate = new DateTime(2023, 05, 01);

            // Create 1 file that already exists at destination
            var academyFolderFiles = RandomGen.GoogleDriveFiles(filesValidity: true, count: 1);
            academyFolderFiles.First().CreatedTime = referenceDate - TimeSpan.FromDays(6);

            // Mapping of file names in source folder to corresponding file names in destination folder
            var nameChangeRegister = new Dictionary<string, string>() {
                { academyFolderFiles.First().Name, "HousingBenefitFile20230501.dat" },
            };

            var destinationFolderFiles = new List<File>
            {
                RandomGen
                    .Build<File>()
                    .With(copiedFile => copiedFile.Name, nameChangeRegister[academyFolderFiles.First().Name])
                    .Create()
            };

            var academyFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders);

            var destinationFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.HousingBenefitFileLabel)))
                .ReturnsAsync(destinationFolders);

            var academyFolderGId = academyFolders.First().GoogleIdentifier;
            var destinationFolderGId = destinationFolders.First().GoogleIdentifier;

            _mockGoogleClientService
                .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolderGId), It.IsAny<string>()))
                .ReturnsAsync(academyFolderFiles);

            _mockGoogleClientService
                .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == destinationFolderGId), It.IsAny<string>()))
                .ReturnsAsync(destinationFolderFiles);

            // act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            // Does not copy file and throws exception that no valid files to copy were found
            _mockGoogleClientService.Verify(
                g => g.CopyFileInDrive(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never
            );

            var expectedErrorMsg =
                "Expected 1 file to copy from the Academy Folder(s) " +
                "* directories, but found none.";
            await useCaseCall.Should().ThrowAsync<Exception>().WithMessage(expectedErrorMsg);
        }

        // If No academy folders are found...
        [Fact]
        public async Task UCThrowsNoFileSettingsFoundExceptionWhenNoAcademyFileSettingsAreFound()
        {
            // arrange
            var expectedErrorMessage = $"No file settings with label: '{ConstantsGen.AcademyFileFolderLabel}' were found.";

            var academyFolders = new List<GoogleFileSettingDomain>();

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders);

            // act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            await useCaseCall.Should().ThrowAsync<Exception>().WithMessage(expectedErrorMessage);

            _mockBatchLogGateway.Verify(g => g.SetToSuccessAsync(It.IsAny<long>()), Times.Never);
        }

        // If No destination folders are found...
        [Fact]
        public async Task UCThrowsNoFileSettingsFoundExceptionWhenNoHousingBenefitFileSettingsAreFound()
        {
            // arrange
            var expectedErrorMessage = $"No file settings with label: '{ConstantsGen.HousingBenefitFileLabel}' were found.";

            var destinationFolders = new List<GoogleFileSettingDomain>();

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.HousingBenefitFileLabel)))
                .ReturnsAsync(destinationFolders);

            // act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            await useCaseCall.Should().ThrowAsync<Exception>().WithMessage(expectedErrorMessage);

            _mockBatchLogGateway.Verify(g => g.SetToSuccessAsync(It.IsAny<long>()), Times.Never);
        }

        // If No Academy Files Are found
        [Fact]
        public async Task UCThrowsFileNotFoundExceptionWhenNoAcademyFilesAreFound()
        {
            // arrange
            var expectedErrorMessage = $"No files were found within the '{ConstantsGen.AcademyFileFolderLabel}' label directories.";

            var academyFolders = RandomGen.CreateMany<GoogleFileSettingDomain>();

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders);

            // act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            await useCaseCall.Should().ThrowAsync<SIO.FileNotFoundException>().WithMessage(expectedErrorMessage);

            _mockBatchLogGateway.Verify(g => g.SetToSuccessAsync(It.IsAny<long>()), Times.Never);
        }

        // UC copies a valid academy file into every target directory
        [Fact]
        public async Task UCCallsCopiesValidAcademyFileToEveryTargetHousingBenefitDirectory()
        {
            /*
                Here we're assuming that neither target directory already contains the files. The functionality
                for excluding the files that already exist is tested by a dedicated test above.
            */

            // arrange
            var academyFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);
            var academyFiles = RandomGen.GoogleDriveFiles(filesValidity: true, count: 1);

            var destinationFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders);

            _mockGoogleClientService
                    .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolders.First().GoogleIdentifier), It.IsAny<string>()))
                    .ReturnsAsync(academyFiles);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.HousingBenefitFileLabel)))
                .ReturnsAsync(destinationFolders);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            destinationFolders.ForEach(destinationFolder =>
                _mockGoogleClientService.Verify(g =>
                    g.CopyFileInDrive(
                        It.Is<string>(s => s == academyFiles.First().Id),
                        It.Is<string>(s => s == destinationFolder.GoogleIdentifier),
                        It.IsAny<string>()
                    ),
                    Times.Once
                )
            );
        }

        // UC copies all valid files with the correct new file names
        [Fact]
        public async Task UCCopiesValidAcademyFilesWithTheCorrectNewCalculatedName()
        {
            // arrange
            var referenceDate = new DateTime(2023, 05, 01);

            var academyFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);
            var academyFolderFiles = RandomGen.GoogleDriveFiles(filesValidity: true, count: 1);

            var academyFile = academyFolderFiles[0];
            academyFile.Name = "06052022_Something_Academy_20052022";
            academyFile.CreatedTime = referenceDate - TimeSpan.FromDays(2);

            // Expected new name for the academy file
            var renamedFileName = "HousingBenefitFile20230501.dat";

            var nameChangeRegister = new Dictionary<string, string>() {
                { academyFile.Name, renamedFileName },
            };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders);

            _mockGoogleClientService
                    .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolders.First().GoogleIdentifier), It.IsAny<string>()))
                    .ReturnsAsync(academyFolderFiles);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleClientService.Verify(g =>
                g.CopyFileInDrive(
                    It.Is<string>(s => s == academyFile.Id),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == nameChangeRegister[academyFile.Name])
                )
            );
        }

        // UC calls batch log on success
        [Fact]
        public async Task UCCallsBatchLogGWSetToSuccessMethodWhenTheProcessStepGetsExecutedWithoutFailure()
        {
            // arrange
            var batchLog = RandomGen.BatchLogDomain();

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(batchLog);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockBatchLogGateway.Verify(g =>
                g.SetToSuccessAsync(It.Is<long>(l => l == batchLog.Id)),
                Times.Once
            );
        }

        // UC returns the correct response on success
        [Fact]
        public async Task UCReturnsTheStepResponseWithCorrectDataUponSuccessfulProcessStepExecution()
        {
            // arrange
            var expectedNextStepExecutionTime = DateTime.Now.AddSeconds(_nextStepDelayDurationInSeconds);
            var assertionPrecissionInMilliseconds = 1000;
            var expectedContinuationFlag = true;

            // act
            var stepResponse = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            stepResponse.Continue.Should().Be(expectedContinuationFlag);
            stepResponse.NextStepTime.Should().BeCloseTo(expectedNextStepExecutionTime, assertionPrecissionInMilliseconds);
        }

        // If Google GW throws...
        /*
            Upon calling the 'GetFilesInDrive(id)' method, when the given folder id does not exist, the google client service returns a file not found error.
            This is possibly the only scenario, where this method can throw an exception.

            This is because this method is not able to distinguish between seeing no files and not having access to see
            files within the folder. In both such cases, it returns an empty list, hence, no error thrown by the gateway.
        */
        [Fact]
        public async Task WhenGoogleClientServiceGetFilesInDriveMethodThrowsAFileNotFoundExceptionThenUCLogsItViaBatchLogErrorGWAndRethrowsThatSameException()
        {
            // arrange
            var batchLog = RandomGen.BatchLogDomain();

            var folderNotFoundException = ErrorGen.FileNotFoundException();

            _mockGoogleClientService
                .Setup(g => g.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(folderNotFoundException);

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(batchLog);

            // act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            await useCaseCall.Should().ThrowAsync<GoogleApiException>().WithMessage(folderNotFoundException.Message);

            _mockBatchLogErrorGateway.Verify(g =>
                g.CreateAsync(
                    It.Is<long>(l => l == batchLog.Id),
                    It.Is<string>(s => s == "ERROR"),
                    It.Is<string>(s => s == folderNotFoundException.Message)), // really?
                Times.Once
            );
        }

        // CopyFileInDrive file not found
        /*
            The 'CopyFileInDrive' throws the same File Not Found error within these 4 scenarios:
            1. Copied file doesn't exist, but destination folder does exist.
            2. Copied file does exist, but the destination folder doesn't exist.
            3. Copied file does exist, but permissions don't allow target file access.
            4. Copied file does exist, but permissions don't allow destination folder access.
        */
        [Fact]
        public async Task WhenGoogleClientServiceCopyFileInDriveMethodThrowsAFileNotFoundExceptionThenUCLogsItViaBatchLogErrorGWAndRethrowsThatSameException()
        {
            // arrange
            var batchLog = RandomGen.BatchLogDomain();

            var fileNotFoundException = ErrorGen.FileNotFoundException();

            _mockGoogleClientService
                .Setup(g => g.CopyFileInDrive(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(fileNotFoundException);

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(batchLog);

            // act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            await useCaseCall.Should().ThrowAsync<GoogleApiException>().WithMessage(fileNotFoundException.Message);

            _mockBatchLogErrorGateway.Verify(g =>
                g.CreateAsync(
                    It.Is<long>(l => l == batchLog.Id),
                    It.Is<string>(s => s == "ERROR"),
                    It.Is<string>(s => s == fileNotFoundException.Message)),
                Times.Once
            );
        }

        // CopyFileInDrive storage quota exceeded
        [Fact]
        public async Task WhenGoogleClientServiceCopyFileInDriveMethodThrowsStorageQuotaExceededThenUCLogsItViaBatchLogErrorGWAndRethrowsThatSameException()
        {
            // arrange
            var batchLog = RandomGen.BatchLogDomain();

            var copyingIsForbiddenException = ErrorGen.StorageQuotaExceeded();

            _mockGoogleClientService
                .Setup(g => g.CopyFileInDrive(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(copyingIsForbiddenException);

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(batchLog);

            // act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            await useCaseCall.Should().ThrowAsync<GoogleApiException>().WithMessage(copyingIsForbiddenException.Message);

            _mockBatchLogErrorGateway.Verify(g =>
                g.CreateAsync(
                    It.Is<long>(l => l == batchLog.Id),
                    It.Is<string>(s => s == "ERROR"),
                    It.Is<string>(s => s == copyingIsForbiddenException.Message)),
                Times.Once
            );
        }


        // If File Setting GW throws...
        [Fact]
        public async Task WhenGoogleFileSettingsGWGetSettingsByLabelMethodThrowsAnyExceptionThenUCLogsItViaBatchLogErrorGWAndRethrowsThatSameException()
        {
            // arrange
            var expectedMessage = "Timeout error - timed out while attempting to connect to establish a database connection.";
            var expectedException = new TimeoutException(expectedMessage);

            var batchLog = RandomGen.BatchLogDomain();

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(batchLog);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ThrowsAsync(expectedException);

            // act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            await useCaseCall.Should().ThrowAsync<TimeoutException>().WithMessage(expectedMessage);

            _mockBatchLogErrorGateway.Verify(g =>
                g.CreateAsync(
                    It.Is<long>(l => l == batchLog.Id),
                    It.Is<string>(s => s == "ERROR"),
                    It.Is<string>(s => s == expectedMessage)
                ),
                Times.Once
            );
        }
    }
}
