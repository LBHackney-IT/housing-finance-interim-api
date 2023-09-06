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

public class LoadActionDiaryUseCaseTests
{
    private readonly Mock<IBatchLogGateway> _mockBatchLogGateway;
    private readonly Mock<IBatchLogErrorGateway> _mockBatchLogErrorGateway;
    private readonly Mock<IActionDiaryGateway> _mockActionDiaryGateway;
    private readonly Mock<IGoogleFileSettingGateway> _mockGoogleFileSettingGateway;
    private readonly Mock<IGoogleClientService> _mockGoogleClientService;

    private readonly LoadActionDiaryUseCase _classUnderTest;

    public LoadActionDiaryUseCaseTests()
    {
        _mockBatchLogGateway = new Mock<IBatchLogGateway>();
        _mockBatchLogErrorGateway = new Mock<IBatchLogErrorGateway>();
        _mockActionDiaryGateway = new Mock<IActionDiaryGateway>();
        _mockGoogleFileSettingGateway = new Mock<IGoogleFileSettingGateway>();
        _mockGoogleClientService = new Mock<IGoogleClientService>();

        var waitDurationEnvVar = 30;
        Environment.SetEnvironmentVariable("WAIT_DURATION", waitDurationEnvVar.ToString());

        _classUnderTest = new LoadActionDiaryUseCase
            (
                _mockBatchLogGateway.Object,
                _mockBatchLogErrorGateway.Object,
                _mockActionDiaryGateway.Object,
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
            .Setup(x => x.ReadSheetToEntitiesAsync<ActionDiaryAuxDomain>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>())
            ).ReturnsAsync(RandomGen.CreateMany<ActionDiaryAuxDomain>(quantity: 1).ToList());
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

    [Fact]
    public async Task LoadsActionDiaryWhenAllOK()
    {
        // Act
        var useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        var result = await useCaseCall().ConfigureAwait(false);
        result.Continue.Should().BeTrue();
        
        _mockActionDiaryGateway.Verify(gw =>
                gw.ClearActionDiaryAuxiliary(), Times.Once);
        _mockActionDiaryGateway.Verify(gw =>
                gw.CreateBulkAsync(It.IsAny<IList<ActionDiaryAuxDomain>>()),
            Times.Once);
        _mockActionDiaryGateway.Verify(gw =>
                gw.LoadActionDiary(), Times.Once);
    }

    [Fact]
    public async Task ThrowsExceptionWhenFailingToClearAuxTable()
    {
        // Arrange
        var testException = new Exception("Test exception");
        
        _mockBatchLogGateway
            .Setup(g => g.CreateAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(testException);

        // Act
        var useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        await useCaseCall.Should().ThrowAsync<Exception>().WithMessage(testException.Message).ConfigureAwait(false);
        _mockBatchLogGateway.Verify(x => x.SetToSuccessAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task ThrowsExceptionWhenNoFileSettingsFound()
    {
        // When no items in GoogleFileSetting table containing spreadsheet ID to read from for the load tenancy agreement label
        var emptyGoogleFileSettingList = new List<GoogleFileSettingDomain>();

        // Arrange
        _mockGoogleFileSettingGateway
            .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
            .ReturnsAsync(emptyGoogleFileSettingList);

        // Act
        var useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        await useCaseCall.Should().ThrowAsync<GoogleFileSettingNotFoundException>().ConfigureAwait(false);
    }

    [Fact]
    public async Task DoesNotAttemptToLoadDatabaseWithEmptySheets()
    {
        var emptySheetEntityList = new List<ActionDiaryAuxDomain>();
        const string blankSheetName = "Blank";

        // Arrange
        _mockGoogleClientService
            .Setup(service => service.ReadSheetToEntitiesAsync<ActionDiaryAuxDomain>(
                It.IsAny<string>(),
                blankSheetName,
                It.IsAny<string>())
            )
            .ReturnsAsync(emptySheetEntityList);

        // Act
        await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        _mockActionDiaryGateway.Verify(gw =>
                gw.ClearActionDiaryAuxiliary(), Times.Never);
        _mockActionDiaryGateway.Verify(gw =>
                gw.CreateBulkAsync(It.IsAny<IList<ActionDiaryAuxDomain>>()),
            Times.Never);
        _mockActionDiaryGateway.Verify(gw =>
                gw.LoadActionDiary(), Times.Never);
    }
}