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
using HousingFinanceInterimApi.V1.Exceptions;

namespace HousingFinanceInterimApi.Tests.V1.UseCase;

public class LoadSuspenseHousingBenefitTransactionsUseCaseTests
{
    private readonly Mock<IBatchLogGateway> _mockBatchLogGateway;
    private readonly Mock<IBatchLogErrorGateway> _mockBatchLogErrorGateway;
    private readonly Mock<ISuspenseAccountsGateway> _mockSuspenseAccountsGateway;
    private readonly Mock<IGoogleFileSettingGateway> _mockGoogleFileSettingGateway;
    private readonly Mock<IGoogleClientService> _mockGoogleClientService;

    private LoadSuspenseCashTransactionsUseCase _classUnderTest;

    public LoadSuspenseHousingBenefitTransactionsUseCaseTests()
    {
        _mockBatchLogGateway = new Mock<IBatchLogGateway>();
        _mockBatchLogErrorGateway = new Mock<IBatchLogErrorGateway>();
        _mockSuspenseAccountsGateway = new Mock<ISuspenseAccountsGateway>();
        _mockGoogleFileSettingGateway = new Mock<IGoogleFileSettingGateway>();
        _mockGoogleClientService = new Mock<IGoogleClientService>();

        var waitDurationEnvVar = 30;
        Environment.SetEnvironmentVariable("WAIT_DURATION", waitDurationEnvVar.ToString());

        _classUnderTest = new LoadSuspenseCashTransactionsUseCase
            (
                _mockBatchLogGateway.Object,
                _mockBatchLogErrorGateway.Object,
                _mockSuspenseAccountsGateway.Object,
                _mockGoogleFileSettingGateway.Object,
                _mockGoogleClientService.Object
            );

        _mockBatchLogGateway
            .Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(RandomGen.BatchLogDomain());

        _mockBatchLogGateway
            .Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(RandomGen.BatchLogDomain());

        _mockSuspenseAccountsGateway
            .Setup(x => x.GetCashSuspenseTransactions())
            .ReturnsAsync(RandomGen.CreateMany<SuspenseTransactionAuxDomain>(quantity: 1));

        var fileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);
        _mockGoogleFileSettingGateway
            .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
            .ReturnsAsync(fileSettings);

        _mockGoogleClientService
            .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(RandomGen.CreateMany<File>(1));

        _mockGoogleClientService
            .Setup(x => x.ReadSheetToEntitiesAsync<SuspenseTransactionAuxDomain>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>())
            ).ReturnsAsync(RandomGen.CreateMany<SuspenseTransactionAuxDomain>(quantity: 1));
    }

    [Fact]
    public async void ThrowsIfNoGoogleFileSettingsFound()
    {
        // Arrange
        _mockGoogleFileSettingGateway
            .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
            .ReturnsAsync(new List<GoogleFileSettingDomain>());

        // Act
        var useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        await useCaseCall.Should().ThrowAsync<GoogleFileSettingNotFoundException>().ConfigureAwait(false);
    }

    [Fact]
    public async Task LoadToTablesIfSpreadsheetSuspenseTransactionsFound()
    {
        // Act
        await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        _mockSuspenseAccountsGateway.Verify(x => x.ClearSuspenseTransactionsAuxAuxiliary(), Times.Once);
        _mockSuspenseAccountsGateway.Verify(x => x.CreateBulkAsync(It.IsAny<List<SuspenseTransactionAuxDomain>>(), It.IsAny<string>()), Times.Once);
        _mockSuspenseAccountsGateway.Verify(x => x.LoadCashSuspenseTransactions(), Times.Once);
    }

    [Fact]
    public async Task RetrievesSuspAccsAndUpdatesSpreadsheet()
    {
        // Arrange
        var suspenseTransactionsCount = 4;
        _mockSuspenseAccountsGateway
            .Setup(x => x.GetCashSuspenseTransactions())
            .ReturnsAsync(RandomGen.CreateMany<SuspenseTransactionAuxDomain>(quantity: suspenseTransactionsCount));

        // Act
        await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        _mockSuspenseAccountsGateway.Verify(x => x.GetCashSuspenseTransactions(), Times.Once);
        _mockGoogleClientService.Verify(x => x.UpdateSheetAsync(It.Is<List<IList<object>>>(x => x.Count == suspenseTransactionsCount + 1), // plus header row
                                                                It.IsAny<string>(),
                                                                It.IsAny<string>(),
                                                                It.IsAny<string>(),
                                                                true), Times.Once);
    }

    [Fact]
    public async Task ReturnsStepResponseWhenAllOK()
    {
        // Act
        var useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        await useCaseCall.Should().NotThrowAsync().ConfigureAwait(false);

        var stepResponse = await useCaseCall().ConfigureAwait(false);
        stepResponse.Continue.Should().BeTrue();
    }
}
