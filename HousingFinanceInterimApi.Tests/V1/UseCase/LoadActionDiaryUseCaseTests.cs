using System;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Exceptions;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Moq;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Google.Apis.Drive.v3.Data;

namespace HousingFinanceInterimApi.Tests.V1.UseCase;

public class LoadActionDiaryUseCaseTests
{
    private readonly Mock<IBatchLogGateway> _mockBatchLogGateway;
    private readonly Mock<IBatchLogErrorGateway> _mockBatchLogErrorGateway;
    private readonly Mock<IActionDiaryGateway> _mockActionDiaryGateway;
    private readonly Mock<IGoogleFileSettingGateway> _mockGoogleFileSettingGateway;
    private readonly Mock<IGoogleClientService> _mockGoogleClientService;
    private List<ActionDiaryAuxDomain> _sheetEntities;

    private readonly ILoadActionDiaryUseCase _classUnderTest;

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

        var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1);
        _mockGoogleFileSettingGateway
            .Setup(g => g.GetSettingsByLabel(It.IsAny<string>()))
            .ReturnsAsync(rentPositionFileSettings);

        _mockGoogleClientService
            .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(RandomGen.CreateMany<File>(1));

        _sheetEntities = RandomGen.CreateMany<ActionDiaryAuxDomain>(quantity: 1);
        _mockGoogleClientService
            .Setup(x => x.ReadSheetToEntitiesAsync<ActionDiaryAuxDomain>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>())
            ).ReturnsAsync(_sheetEntities);
    }

    [Fact]
    public async Task ReturnsStepResponseWhenAllOK()
    {
        // Act
        var stepResponse = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);
        // Assert 
        var waitDurationEnvVar = Environment.GetEnvironmentVariable("WAIT_DURATION");
        var expectedNextStepTime = DateTime.UtcNow.AddSeconds(int.Parse(waitDurationEnvVar));

        stepResponse.Continue.Should().BeTrue();
        stepResponse.NextStepTime.Should().BeCloseTo(expectedNextStepTime);
    }

    [Fact]
    public async Task LoadsActionDiaryWhenAllOK()
    {
        // Act
        var result = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        result.Continue.Should().BeTrue();

        _mockActionDiaryGateway.Verify(gw => gw.ClearActionDiaryAuxiliary(), Times.Once);
        _mockActionDiaryGateway.Verify(gw => gw.CreateBulkAsync(_sheetEntities), Times.Once);
        _mockActionDiaryGateway.Verify(gw => gw.LoadActionDiary(), Times.Once);
    }

    [Fact]
    public async Task ThrowsExceptionWhenCreateBatchFails()
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
    public async Task ErrorLoggedWhenClearAuxTableThrows()
    {
        // Arrange
        var testException = new Exception("Test exception");

        _mockActionDiaryGateway
            .Setup(g => g.ClearActionDiaryAuxiliary())
            .ThrowsAsync(testException);

        // Act
        var useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        await useCaseCall.Should().ThrowAsync<Exception>().WithMessage(testException.Message).ConfigureAwait(false);
        _mockBatchLogErrorGateway.Verify(x => x.CreateAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ErrorLoggedWhenCreateBulkInsertThrows()
    {
        // Arrange
        var testException = new Exception("Test exception");

        _mockActionDiaryGateway
            .Setup(g => g.CreateBulkAsync(_sheetEntities))
            .ThrowsAsync(testException);

        // Act
        var useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        await useCaseCall.Should().ThrowAsync<Exception>().WithMessage(testException.Message).ConfigureAwait(false);
        _mockBatchLogErrorGateway.Verify(x => x.CreateAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ErrorLoggedWhenMergeActionDiaryThrows()
    {
        // Arrange
        var testException = new Exception("Test exception");

        _mockActionDiaryGateway
            .Setup(g => g.LoadActionDiary())
            .ThrowsAsync(testException);

        // Act
        var useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        await useCaseCall.Should().ThrowAsync<Exception>().WithMessage(testException.Message).ConfigureAwait(false);
        _mockBatchLogErrorGateway.Verify(x => x.CreateAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task DoesNotAttemptToLoadDatabaseWithEmptySheets()
    {
        // Arrange
        var emptySheetEntityList = new List<ActionDiaryAuxDomain>();

        _mockGoogleClientService
            .Setup(service => service.ReadSheetToEntitiesAsync<ActionDiaryAuxDomain>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>())
            )
            .ReturnsAsync(emptySheetEntityList);

        // Act
        await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

        // Assert
        _mockActionDiaryGateway.Verify(gw => gw.ClearActionDiaryAuxiliary(), Times.Never);
        _mockActionDiaryGateway.Verify(gw => gw.CreateBulkAsync(_sheetEntities), Times.Never);
        _mockActionDiaryGateway.Verify(gw => gw.LoadActionDiary(), Times.Never);
    }
}
