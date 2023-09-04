using System;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using Moq;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Google.Apis.Drive.v3.Data;
using HousingFinanceInterimApi.Tests.V1.UseCase.Exceptions;

namespace HousingFinanceInterimApi.Tests.V1.UseCase;

public class LoadTenancyAgreementUseCaseTests
{
        private readonly Mock<IBatchLogGateway> _mockBatchLogGateway;
        private readonly Mock<IBatchLogErrorGateway> _mockBatchLogErrorGateway;
        private readonly Mock<ITenancyAgreementGateway> _mockTenancyAgreementGateway;
        private readonly Mock<IGoogleFileSettingGateway> _mockGoogleFileSettingGateway;
        private readonly Mock<IGoogleClientService> _mockGoogleClientService;

        private LoadTenancyAgreementUseCase _classUnderTest;

        public LoadTenancyAgreementUseCaseTests()
        {
            _mockBatchLogGateway = new Mock<IBatchLogGateway>();
            _mockBatchLogErrorGateway = new Mock<IBatchLogErrorGateway>();
            _mockTenancyAgreementGateway = new Mock<ITenancyAgreementGateway>();
            _mockGoogleFileSettingGateway = new Mock<IGoogleFileSettingGateway>();
            _mockGoogleClientService = new Mock<IGoogleClientService>();

            var waitDurationEnvVar = 30;
            Environment.SetEnvironmentVariable("WAIT_DURATION", waitDurationEnvVar.ToString());

            _classUnderTest = new LoadTenancyAgreementUseCase
                (
                    _mockBatchLogGateway.Object,
                    _mockBatchLogErrorGateway.Object,
                    _mockTenancyAgreementGateway.Object,
                    _mockGoogleFileSettingGateway.Object,
                    _mockGoogleClientService.Object
                );

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(rentPositionFileSettings);

            _mockGoogleClientService
                .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(RandomGen.CreateMany<File>(1).ToList());

            _mockGoogleClientService
                .Setup(x => x.ReadSheetToEntitiesAsync<TenancyAgreementAuxDomain>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())
                ).ReturnsAsync(RandomGen.CreateMany<TenancyAgreementAuxDomain>(quantity: 1).ToList());
        }

        [Fact]
        public async Task ReturnsStepResponseWhenAllOK()
        {
            // Act
            var useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().NotThrowAsync<Exception>().ConfigureAwait(false);

            var stepResponse = await useCaseCall().ConfigureAwait(false);
            stepResponse.Continue.Should().BeTrue();
        }

        [Fact]
        public async Task RunsAllUpdateFunctionsForEachRentGroupWhenAllOK()
        {
            // Act
            var useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            var result = await useCaseCall().ConfigureAwait(false);
            result.Continue.Should().BeTrue();

            var countOfRentGroups = Enum.GetValues(typeof(RentGroup)).Length;
            var firstRentGroup = Enum.GetValues(typeof(RentGroup)).GetValue(0)?.ToString();

            _mockTenancyAgreementGateway.Verify(gw =>
                    gw.CreateBulkAsync(It.IsAny<IList<TenancyAgreementAuxDomain>>(), firstRentGroup),
                Times.Once);

            _mockTenancyAgreementGateway.Verify(gw =>
                    gw.ClearTenancyAgreementAuxiliary(),
                Times.Exactly(countOfRentGroups));
            _mockTenancyAgreementGateway.Verify(gw =>
                    gw.CreateBulkAsync(It.IsAny<IList<TenancyAgreementAuxDomain>>(), It.IsAny<string>()),
                Times.Exactly(countOfRentGroups));
            _mockTenancyAgreementGateway.Verify(gw =>
                    gw.RefreshTenancyAgreement(It.IsAny<long>()),
                Times.Exactly(countOfRentGroups));
        }

        [Fact]
        public async Task ThrowsExceptionWhenFailingToCreateBatchLogs()
        {
            var batchLogException = new Exception("Test exception");

            // Arrange
            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ThrowsAsync(batchLogException);

            // Act
            var useCaseCall = async () =>  await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().ThrowAsync<Exception>().ConfigureAwait(false);
            _mockBatchLogGateway.Verify(x => x.SetToSuccessAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task ThrowsExceptionWhenNoRentPositionFileSettingsFound()
        {
            // When no items in GoogleFileSetting table containing spreadsheet ID to read from for the load tenancy agreement label
            var emptyGoogleFileSettingList = new List<GoogleFileSettingDomain>();

            // Arrange
            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
                .ReturnsAsync(emptyGoogleFileSettingList);

            // Act
            var useCaseCall = async () =>  await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().ThrowAsync<GoogleFileSettingNotFoundException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task ThrowsUncaughtExceptionWhenFailingToReadSpreadsheet()
        {
            var emptySheetEntityList = new List<TenancyAgreementAuxDomain>();

            // Arrange
            _mockGoogleClientService
                .Setup(service => service.ReadSheetToEntitiesAsync<TenancyAgreementAuxDomain>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())
                ).ReturnsAsync(emptySheetEntityList);

            // Act
            var useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().ThrowAsync<AggregateException>().ConfigureAwait(false);
        }
}
