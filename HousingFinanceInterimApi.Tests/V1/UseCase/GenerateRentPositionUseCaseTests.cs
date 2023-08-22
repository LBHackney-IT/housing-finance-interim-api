using System;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using Moq;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Bogus.DataSets;
using Google.Apis.Drive.v3.Data;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class GenerateRentPositionUseCaseTests
    {
        private readonly Mock<IRentPositionGateway> _mockRentPositionGateway;
        private readonly Mock<IBatchLogGateway> _mockBatchLogGateway;
        private readonly Mock<IBatchLogErrorGateway> _mockBatchLogErrorGateway;
        private readonly Mock<IGoogleFileSettingGateway> _mockGoogleFileSettingGateway;
        private readonly Mock<IGoogleClientService> _mockGoogleClientService;

        private GenerateRentPositionUseCase _classUnderTest;

        public GenerateRentPositionUseCaseTests()
        {
            _mockRentPositionGateway = new Mock<IRentPositionGateway>();
            _mockBatchLogGateway = new Mock<IBatchLogGateway>();
            _mockBatchLogErrorGateway = new Mock<IBatchLogErrorGateway>();
            _mockGoogleFileSettingGateway = new Mock<IGoogleFileSettingGateway>();
            _mockGoogleClientService = new Mock<IGoogleClientService>();

            var waitDurationEnvVar = 30;
            Environment.SetEnvironmentVariable("WAIT_DURATION", waitDurationEnvVar.ToString());

            _classUnderTest = new GenerateRentPositionUseCase
                (
                    _mockRentPositionGateway.Object,
                    _mockBatchLogGateway.Object,
                    _mockBatchLogErrorGateway.Object,
                    _mockGoogleFileSettingGateway.Object,
                    _mockGoogleClientService.Object
                );
        }

        [Fact]
        public async Task ExecuteAsyncThrowsErrorWhenFileSettingIsRentPositionLabelAndExceptionIsNotNull()
        {
            // Arrange
            var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPosition = ConstantsGen.RentPositionLabel;

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.Is<string>(s => s == rentPosition), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPosition)))
                .ReturnsAsync(rentPositionFileSettings);

            _mockRentPositionGateway
                .Setup(g => g.GetRentPosition())
                .ReturnsAsync(RandomGen.RentPositionCsvRepresentation());

            _mockGoogleClientService
                .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<File>());

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(false);

            // Act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().ThrowAsync<Exception>().WithMessage("Failed to upload to Rent Position folder (Qlik)").ConfigureAwait(false);
        }

        [Fact]
        public async Task ExecuteAsyncThrowsErrorWhenFileSettingIsRentPositionBkpLabelAndExceptionIsNotNull()
        {
            // Arrange
            var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPositionBkpFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPosition = ConstantsGen.RentPositionLabel;
            var rentPositionBkp = ConstantsGen.RentPositionBkpLabel;

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.Is<string>(s => s == rentPosition), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPosition)))
                .ReturnsAsync(rentPositionFileSettings);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPositionBkp)))
                .ReturnsAsync(rentPositionBkpFileSettings);

            _mockRentPositionGateway
                .Setup(g => g.GetRentPosition())
                .ReturnsAsync(RandomGen.RentPositionCsvRepresentation());

            _mockGoogleClientService
                .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<File>());

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionBkpFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(false);

            // Act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().ThrowAsync<Exception>().WithMessage("Failed to upload to Rent Position folder (Backup)").ConfigureAwait(false);
        }

        [Fact]
        public async Task DeletesFilesOlderThan7Days()
        {
            // Arrange
            var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPositionBkpFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPosition = ConstantsGen.RentPositionLabel;
            var rentPositionBkp = ConstantsGen.RentPositionBkpLabel;
            var fileList = RandomGen.CreateMany<File>(quantity: 2).ToList();
            var fileToBeDeleted = fileList.First();
            var fileToNotBeDeleted = fileList.Last();
            fileToBeDeleted.CreatedTime = new DateTime(2020, 1, 1);
            fileToNotBeDeleted.CreatedTime = DateTime.Today.AddDays(-2);

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.Is<string>(s => s == rentPosition), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPosition)))
                .ReturnsAsync(rentPositionFileSettings);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPositionBkp)))
                .ReturnsAsync(rentPositionBkpFileSettings);

            _mockRentPositionGateway
                .Setup(g => g.GetRentPosition())
                .ReturnsAsync(RandomGen.RentPositionCsvRepresentation());

            _mockGoogleClientService
                .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileList);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionBkpFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            _mockGoogleClientService
                .Setup(x => x.DeleteFileInDrive(
                    It.Is<string>(s => s == rentPositionFileSettings.First().Id.ToString())));

            // Act
            await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            _mockGoogleClientService.Verify(x => x.DeleteFileInDrive(fileToBeDeleted.Id), Times.Once);
            _mockGoogleClientService.Verify(x => x.DeleteFileInDrive(fileToNotBeDeleted.Id), Times.Never);
        }

        [Fact]
        public async Task DoesNotDeleteFilesIfUploadToRentPosFolderFails()
        {
            // Arrange
            const string expectedErrorMessage = "Failed to upload to Rent Position folder (Qlik)";
            var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPositionBkpFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPosition = ConstantsGen.RentPositionLabel;
            var rentPositionBkp = ConstantsGen.RentPositionBkpLabel;
            var fileList = RandomGen.CreateMany<File>(quantity: 2).ToList();
            var fileToBeDeleted = fileList.First();
            var fileToNotBeDeleted = fileList.Last();
            fileToBeDeleted.CreatedTime = new DateTime(2020, 6, 4);
            fileToNotBeDeleted.CreatedTime = DateTime.Today.AddDays(-2);

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.Is<string>(s => s == rentPosition), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPosition)))
                .ReturnsAsync(rentPositionFileSettings);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPositionBkp)))
                .ReturnsAsync(rentPositionBkpFileSettings);

            _mockRentPositionGateway
                .Setup(g => g.GetRentPosition())
                .ReturnsAsync(RandomGen.RentPositionCsvRepresentation());

            _mockGoogleClientService
                .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileList);

            // Upload to Rent Position folder fails
            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(false);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionBkpFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);


            // Act
            // Check it throws exception
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().ThrowAsync<Exception>().WithMessage(expectedErrorMessage).ConfigureAwait(false);
            _mockGoogleClientService.Verify(x => x.DeleteFileInDrive(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DoesNotDeleteBackupFilesIfUploadToRentPosBackupFolderFails()
        {
            // Arrange
            const string expectedErrorMessage = "Failed to upload to Rent Position folder (Backup)";
            var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPositionBkpFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPosition = ConstantsGen.RentPositionLabel;
            var rentPositionBkp = ConstantsGen.RentPositionBkpLabel;
            var fileList = RandomGen.CreateMany<File>(quantity: 2).ToList();
            fileList.First().CreatedTime = DateTime.Today.AddDays(-10);
            fileList.Last().CreatedTime = DateTime.Today.AddDays(-2);

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.Is<string>(s => s == rentPosition), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPosition)))
                .ReturnsAsync(rentPositionFileSettings);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPositionBkp)))
                .ReturnsAsync(rentPositionBkpFileSettings);

            _mockRentPositionGateway
                .Setup(g => g.GetRentPosition())
                .ReturnsAsync(RandomGen.RentPositionCsvRepresentation());

            _mockGoogleClientService
                .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileList);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            // Upload to Rent Position Backup folder fails
            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionBkpFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(false);


            // Act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().ThrowAsync<Exception>().WithMessage(expectedErrorMessage).ConfigureAwait(false);
            _mockGoogleClientService.Verify(x => x.DeleteFileInDrive(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DoesNotDeleteFilesCreatedAtTheEndOfPreviousFinancialYears()
        {
            // Does not delete files created on 31st March
            // Arrange
            var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPositionBkpFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPosition = ConstantsGen.RentPositionLabel;
            var rentPositionBkp = ConstantsGen.RentPositionBkpLabel;
            var fileList = RandomGen.CreateMany<File>(quantity: 3).ToList();
            var fileToBePreserved1 = fileList.First();
            var fileToBePreserved2 = fileList[1];
            var fileToBeDeleted = fileList.Last();


            fileToBePreserved1.CreatedTime = new DateTime(2020, 3, 31); // Tuesday last wd of financial year
            fileToBePreserved2.CreatedTime = DateTime.Today.AddDays(-3); // Less than 1 week old
            fileToBeDeleted.CreatedTime = new DateTime(2020, 4, 1);

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.Is<string>(s => s == rentPosition), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPosition)))
                .ReturnsAsync(rentPositionFileSettings);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPositionBkp)))
                .ReturnsAsync(rentPositionBkpFileSettings);

            _mockRentPositionGateway
                .Setup(g => g.GetRentPosition())
                .ReturnsAsync(RandomGen.RentPositionCsvRepresentation());

            // Returns the test files
            _mockGoogleClientService
                .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileList);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionBkpFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            // Act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().NotThrowAsync<Exception>().ConfigureAwait(false);
            _mockGoogleClientService.Verify(s => s.DeleteFileInDrive(It.Is<string>(s => s == fileToBeDeleted.Id)), Times.Once);
            _mockGoogleClientService.Verify(x => x.DeleteFileInDrive(It.Is<string>(s => s == fileToBePreserved1.Id)), Times.Never);
            _mockGoogleClientService.Verify(x => x.DeleteFileInDrive(It.Is<string>(s => s == fileToBePreserved2.Id)), Times.Never);
        }

        [Fact]
        public async Task DoesNotDeleteFilesCreatedOnLastWeekdayOfPreviousFinancialYears()
        {
            // Arrange
            var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPositionBkpFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPosition = ConstantsGen.RentPositionLabel;
            var rentPositionBkp = ConstantsGen.RentPositionBkpLabel;
            var fileList = RandomGen.CreateMany<File>(quantity: 2).ToList();
            var fileToBePreserved = fileList.First();
            var fileToBeDeleted = fileList.Last();

            fileToBePreserved.CreatedTime = new DateTime(2019, 3, 29); // Friday
            fileToBeDeleted.CreatedTime = new DateTime(2019, 3, 31); // Sunday

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.Is<string>(s => s == rentPosition), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPosition)))
                .ReturnsAsync(rentPositionFileSettings);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPositionBkp)))
                .ReturnsAsync(rentPositionBkpFileSettings);

            _mockRentPositionGateway
                .Setup(g => g.GetRentPosition())
                .ReturnsAsync(RandomGen.RentPositionCsvRepresentation());

            // Returns the test files
            _mockGoogleClientService
                .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileList);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionBkpFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            // Act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().NotThrowAsync<Exception>().ConfigureAwait(false);
            _mockGoogleClientService.Verify(x => x.DeleteFileInDrive(It.Is<string>(s => s == fileToBeDeleted.Id)), Times.Once);
            _mockGoogleClientService.Verify(x => x.DeleteFileInDrive(It.Is<string>(s => s == fileToBePreserved.Id)), Times.Never);
        }

        [Fact]
        public async Task DoesNotDeleteFilesWithNoCreationDateSetOrFound()
        {
            // Arrange
            var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPositionBkpFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPosition = ConstantsGen.RentPositionLabel;
            var rentPositionBkp = ConstantsGen.RentPositionBkpLabel;
            var fileList = RandomGen.CreateMany<File>(quantity: 2).ToList();
            var fileToBePreserved = fileList.First();
            var fileToBeDeleted = fileList.Last();
            fileToBePreserved.CreatedTime = null; // Friday
            fileToBeDeleted.CreatedTime = new DateTime(2019, 3, 31); // Sunday

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.Is<string>(s => s == rentPosition), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPosition)))
                .ReturnsAsync(rentPositionFileSettings);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPositionBkp)))
                .ReturnsAsync(rentPositionBkpFileSettings);

            _mockRentPositionGateway
                .Setup(g => g.GetRentPosition())
                .ReturnsAsync(RandomGen.RentPositionCsvRepresentation());

            // Returns the test files
            _mockGoogleClientService
                .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileList);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionBkpFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            // Act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().NotThrowAsync<Exception>().ConfigureAwait(false);
            _mockGoogleClientService.Verify(x => x.DeleteFileInDrive(It.Is<string>(s => s == fileToBeDeleted.Id)), Times.Once);
            _mockGoogleClientService.Verify(x => x.DeleteFileInDrive(It.Is<string>(s => s == fileToBePreserved.Id)), Times.Never);
        }

        [Fact]
        public async Task ThrowsExceptionIfAnyFilesFailToDelete()
        {
            // Arrange
            var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPositionBkpFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPosition = ConstantsGen.RentPositionLabel;
            var rentPositionBkp = ConstantsGen.RentPositionBkpLabel;
            var fileList = RandomGen.CreateMany<File>(quantity: 3).ToList();
            var fileToBePreserved = fileList.First();
            var fileToBeDeleted1 = fileList[1];
            var fileToBeDeleted2 = fileList.Last();
            fileToBePreserved.CreatedTime = DateTime.Today.AddDays(-2);
            fileToBeDeleted1.CreatedTime = new DateTime(2023, 1, 11);
            fileToBeDeleted2.CreatedTime = new DateTime(2022, 6, 13);

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.Is<string>(s => s == rentPosition), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPosition)))
                .ReturnsAsync(rentPositionFileSettings);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPositionBkp)))
                .ReturnsAsync(rentPositionBkpFileSettings);

            _mockRentPositionGateway
                .Setup(g => g.GetRentPosition())
                .ReturnsAsync(RandomGen.RentPositionCsvRepresentation());

            _mockGoogleClientService
                .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileList);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionBkpFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            _mockGoogleClientService
                .Setup(x => x.DeleteFileInDrive(It.IsAny<string>())).Throws<Exception>();

            // Act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().ThrowAsync<AggregateException>().ConfigureAwait(false);
            _mockGoogleClientService.Verify(x => x.DeleteFileInDrive(It.IsIn(fileToBeDeleted1.Id, fileToBeDeleted2.Id)), Times.Exactly(2));
            _mockGoogleClientService.Verify(x => x.DeleteFileInDrive(It.Is<string>(s => s == fileToBePreserved.Id)), Times.Never);
            _mockGoogleClientService.Verify(x => x.UploadCsvFile(It.IsAny<List<string[]>>(), It.IsAny<string>(), rentPositionFileSettings.First().GoogleIdentifier), Times.Once);
        }
    }
}
