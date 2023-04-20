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
                .ReturnsAsync(academyFolders.ToList());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.HousingBenefitFileLabel)))
                .ReturnsAsync(destinationFolders.ToList());

            // destination folder files
            _mockGoogleClientService.Setup(g => g.GetFilesInDriveAsync(It.IsAny<string>())).ReturnsAsync(new List<File>());

            // source folder files
            _mockGoogleClientService
                .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolders.First().GoogleIdentifier)))
                .ReturnsAsync(academyFolderFiles.ToList());
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
            var academyFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>().ToList();

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFileSettings);

            // Return a couple files to avoid failure
            _mockGoogleClientService
                .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(RandomGen.GoogleDriveFiles(true).ToList());

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            academyFileSettings.ForEach((academyFileSetting) =>
                _mockGoogleClientService.Verify(
                    g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFileSetting.GoogleIdentifier)),
                    Times.Once
                )
            );
        }

        // UC calls google files GET for each destination folder
        [Fact]
        public async Task UCCallsGoogleClientServiceGetFilesInDriveMethodWithEachHousingBenefitDestinationFolderGIdReturnedByGoogleFileSettingGateway()
        {
            // arrange
            var destinationFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>().ToList();
            var expectedGDriveDestIdentifiers = destinationFileSettings.Select(s => s.GoogleIdentifier).ToList();

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.HousingBenefitFileLabel)))
                .ReturnsAsync(destinationFileSettings);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            destinationFileSettings.ForEach(destinationFolder =>
                _mockGoogleClientService.Verify(
                    g => g.GetFilesInDriveAsync(It.Is<string>(s => s == destinationFolder.GoogleIdentifier)),
                    Times.Once
                )
            );
        }

        // It calls the COPY method only for files that don't already exist at destination:
        [Fact]
        public async Task UCCallsGoogleClientServiceCopyFileInDriveMethodForEachValidAcademyFileThatDoesntAlreadyExistAtDestination()
        {
            // arrange
            var academyNewFilesCount = 2;
            var academyAlreadyCopiedFilesCount = 3;

            // Create 2 new files
            var academyFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);
            var academyNewFiles = RandomGen.GoogleDriveFiles(filesValidity: true, count: academyNewFilesCount);

            var newFiles = academyNewFiles as File[] ?? academyNewFiles.ToArray();
            newFiles[0].Name = "07042022_Something_Academy_21042025";
            newFiles[1].Name = "07042022_Something_Academy_03022026";
            newFiles[0].CreatedTime = new DateTime(2023, 4, 1);
            newFiles[1].CreatedTime = new DateTime(2023, 4, 11);


            // Create 3 files that already exist at destination
            var academyAlreadyCopiedFiles = RandomGen.GoogleDriveFiles(filesValidity: true, count: academyAlreadyCopiedFilesCount).ToList();
            academyAlreadyCopiedFiles[0].Name = "07042022_Something_Academy_21042022";
            academyAlreadyCopiedFiles[1].Name = "20012023_Something_Academy_03042023";
            academyAlreadyCopiedFiles[2].Name = "15082022_Something_Academy_29082022";
            academyAlreadyCopiedFiles[0].CreatedTime = new DateTime(2022, 4, 21);
            academyAlreadyCopiedFiles[1].CreatedTime = new DateTime(2022, 2, 3);
            academyAlreadyCopiedFiles[2].CreatedTime = new DateTime(2022, 8, 29);

            var academyFolderFiles = newFiles.Concat(academyAlreadyCopiedFiles);

            // Mapping of file names in source folder to corresponding file names in destination folder
            var nameChangeRegister = new Dictionary<string, string>() {
                { academyAlreadyCopiedFiles[0].Name, "HousingBenefitFile20220425.dat" },
                { academyAlreadyCopiedFiles[1].Name, "OK_HousingBenefitFile20220207.dat" },
                { academyAlreadyCopiedFiles[2].Name, "NOK_HousingBenefitFile20220829.dat" }
            };

            var destinationFolderFiles = academyAlreadyCopiedFiles.Select(fileAtSource =>
            {
                var copiedFileAtDest = RandomGen
                    .Build<File>()
                    .With(copiedFile => copiedFile.Name, nameChangeRegister[fileAtSource.Name])
                    .Create();

                return copiedFileAtDest;
            }).ToList();

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders.ToList());

            var destinationFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.HousingBenefitFileLabel)))
                .ReturnsAsync(destinationFolders.ToList());

            var academyFolderGId = academyFolders.First().GoogleIdentifier;
            var destinationFolderGId = destinationFolders.First().GoogleIdentifier;

            _mockGoogleClientService
                    .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolderGId)))
                    .ReturnsAsync(academyFolderFiles.ToList());

            _mockGoogleClientService
                    .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == destinationFolderGId)))
                    .ReturnsAsync(destinationFolderFiles.ToList());

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            _mockGoogleClientService.Verify(
                g => g.CopyFileInDrive(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(academyNewFilesCount)
            );

            academyNewFiles
                .ToList()
                .ForEach(academyFile =>
                    _mockGoogleClientService.Verify(
                        g => g.CopyFileInDrive(
                            It.Is<string>(s => s == academyFile.Id),
                            It.Is<string>(s => s == destinationFolderGId),
                            It.IsAny<string>()),
                        Times.Once
                    )
                );

            academyAlreadyCopiedFiles
                .ToList()
                .ForEach(existingAcademyFile =>
                    _mockGoogleClientService.Verify(
                        g => g.CopyFileInDrive(
                            It.Is<string>(s => s == existingAcademyFile.Id),
                            It.Is<string>(s => s == destinationFolderGId),
                            It.IsAny<string>()),
                        Times.Never
                    )
                );
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

        [Fact]
        public async Task UCThrowsExceptionWhenAcademyFileCreationTimeIsNull()
        {
            // arrange
            const string FileName = "20042023_Something_Academy_04052023";
            var Parents = new List<string>() { "1234567890", "3141592653" }.AsReadOnly();
            var expectedErrorMessage = $"File {FileName} in folder(s) {String.Join(", ", Parents)} has no creation date";

            var academyFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);
            var validAcademyFolder = RandomGen.GoogleDriveFiles(filesValidity: true);
            var validAcademyFolderArray = validAcademyFolder as File[] ?? validAcademyFolder.ToArray();

            // Set file with null creation time and corresponding expected error message
            validAcademyFolderArray.First().CreatedTime = null;
            validAcademyFolderArray.First().Name = FileName;
            validAcademyFolderArray.First().Parents = Parents;

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders.ToList());

            _mockGoogleClientService
                .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolders.First().GoogleIdentifier)))
                .ReturnsAsync(validAcademyFolderArray.ToList());

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
                .ReturnsAsync(academyFolders.ToList());

            // act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            await useCaseCall.Should().ThrowAsync<SIO.FileNotFoundException>().WithMessage(expectedErrorMessage);

            _mockBatchLogGateway.Verify(g => g.SetToSuccessAsync(It.IsAny<long>()), Times.Never);
        }

        // If no validly named files exist...
        [Fact]
        public async Task UCThrowsFileNotFoundExceptionWhenAllAcademyFilesHaveInvalidNames()
        {
            // arrange
            var expectedErrorMessage = $"No files with valid name were found within the '{ConstantsGen.AcademyFileFolderLabel}' label directories.";

            var academyFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 2);
            var invalidAcademyFilesFolder1 = RandomGen.GoogleDriveFiles(filesValidity: false);
            var invalidAcademyFilesFolder2 = RandomGen.GoogleDriveFiles(filesValidity: false);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders.ToList());

            _mockGoogleClientService
                    .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolders.First().GoogleIdentifier)))
                    .ReturnsAsync(invalidAcademyFilesFolder1.ToList());

            _mockGoogleClientService
                    .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolders.Last().GoogleIdentifier)))
                    .ReturnsAsync(invalidAcademyFilesFolder2.ToList());

            // act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            await useCaseCall.Should().ThrowAsync<SIO.FileNotFoundException>().WithMessage(expectedErrorMessage);

            _mockBatchLogGateway.Verify(g => g.SetToSuccessAsync(It.IsAny<long>()), Times.Never);
        }


        // UC copies all valid files into every target directory
        [Fact]
        public async Task UCCallsCopiesAllValidAcademyFilesToEveryTargetHousingBenefitDirectory()
        {
            /*
                Here we're assuming that neither target directory already contains the files. The functionality
                for excluding the files that already exist is tested by a dedicated test above.
            */

            // arrange
            var academyFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 2);
            var academyFilesFolder1 = RandomGen.GoogleDriveFiles(filesValidity: true);
            var academyFilesFolder2 = RandomGen.GoogleDriveFiles(filesValidity: true);
            var academyFolderFiles = academyFilesFolder1.Concat(academyFilesFolder2).ToList();

            var destinationFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 2).ToList();

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders.ToList());

            _mockGoogleClientService
                    .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolders.First().GoogleIdentifier)))
                    .ReturnsAsync(academyFilesFolder1.ToList());

            _mockGoogleClientService
                    .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolders.Last().GoogleIdentifier)))
                    .ReturnsAsync(academyFilesFolder2.ToList());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.HousingBenefitFileLabel)))
                .ReturnsAsync(destinationFolders);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            destinationFolders.ForEach(destinationFolder =>
                academyFolderFiles.ForEach(validAcademyFile =>
                    _mockGoogleClientService.Verify(g =>
                        g.CopyFileInDrive(
                            It.Is<string>(s => s == validAcademyFile.Id),
                            It.Is<string>(s => s == destinationFolder.GoogleIdentifier),
                            It.IsAny<string>()
                        ),
                        Times.Once
                    )
                )
            );
        }

        // UC copies all valid files with the correct new file names
        [Fact]
        public async Task UCCopiesValidAcademyFilesWithTheCorrectNewCalculatedName()
        {
            // arrange
            var academyFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);
            var academyFolderFiles = RandomGen.GoogleDriveFiles(filesValidity: true, count: 2).ToList();


            academyFolderFiles[0].Name = "06052022_Something_Academy_20052022";
            academyFolderFiles[0].CreatedTime = new DateTime(2022, 2, 20);
            academyFolderFiles[1].Name = "11022023_Something_Academy_25022023";
            academyFolderFiles[1].CreatedTime = new DateTime(2023, 2, 25);

            var newNameFile1 = "HousingBenefitFile20220221.dat";
            var newNameFile2 = "HousingBenefitFile20230227.dat";

            var nameChangeRegister = new Dictionary<string, string>() {
                { academyFolderFiles[0].Name, newNameFile1 },
                { academyFolderFiles[1].Name, newNameFile2 }
            };

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders.ToList());

            _mockGoogleClientService
                    .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolders.First().GoogleIdentifier)))
                    .ReturnsAsync(academyFolderFiles);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            academyFolderFiles.ForEach(academyFile =>
                _mockGoogleClientService.Verify(g =>
                    g.CopyFileInDrive(
                        It.Is<string>(s => s == academyFile.Id),
                        It.IsAny<string>(),
                        It.Is<string>(s => s == nameChangeRegister[academyFile.Name])
                    )
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
                .Setup(g => g.GetFilesInDriveAsync(It.IsAny<string>()))
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

        // UC calls batch log error for each invalid file name
        [Fact]
        public async Task UCCallsBatchLogErrorGWCreateMethodForEachFileWithInvalidName()
        {
            // arrange
            Func<string, string> expectedErrorMessage = fileName => $"Application error. Not possible to copy academy files({fileName})";

            var academyFolders = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);
            var academyFolderValidFiles = RandomGen.GoogleDriveFiles(filesValidity: true, count: 1);
            var academyFolderNotValidFiles = RandomGen.GoogleDriveFiles(filesValidity: false).ToList();
            var academyFolderFiles = academyFolderValidFiles.Concat(academyFolderNotValidFiles);

            var batchLog = RandomGen.BatchLogDomain();

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == ConstantsGen.AcademyFileFolderLabel)))
                .ReturnsAsync(academyFolders.ToList());

            _mockGoogleClientService
                .Setup(g => g.GetFilesInDriveAsync(It.Is<string>(s => s == academyFolders.First().GoogleIdentifier)))
                .ReturnsAsync(academyFolderFiles.ToList());

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(batchLog);

            // act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // assert
            academyFolderNotValidFiles.ForEach(notValidFile =>
                _mockBatchLogErrorGateway.Verify(g =>
                    g.CreateAsync(
                        It.Is<long>(l => l == batchLog.Id),
                        It.Is<string>(s => s == "ERROR"),
                        It.Is<string>(s => s == expectedErrorMessage(notValidFile.Name))
                    ),
                    Times.Once
                )
            );

            _mockBatchLogErrorGateway.VerifyNoOtherCalls();
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
