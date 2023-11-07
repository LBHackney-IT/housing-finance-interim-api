using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Moq;
using Xunit;
using AutoFixture;
using System.Linq;
using HousingFinanceInterimApi.V1.Exceptions;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class LoadDirectDebitUseCaseTest
    {
        private readonly Mock<IBatchLogGateway> _mockBatchLogGateway;
        private readonly Mock<IBatchLogErrorGateway> _mockBatchLogErrorGateway;
        private readonly Mock<IDirectDebitGateway> _mockDirectDebitGateway;
        private readonly Mock<IGoogleFileSettingGateway> _mockGoogleFileSettingGateway;
        private readonly Mock<IGoogleClientService> _mockGoogleClientService;

        private readonly Fixture _fixture = new();
        private readonly ILoadDirectDebitUseCase _classUnderTest;

        private readonly double _waitDuration = 30;
        private readonly string _directDebitLabel = "DirectDebit";
        private readonly string _sheetId = "google_identifier";
        private readonly string[] _sheetNames = "Rent;LH".Split(";");
        private readonly string _sheetRange = "A:C";
        public LoadDirectDebitUseCaseTest()
        {
            Environment.SetEnvironmentVariable("WAIT_DURATION", _waitDuration.ToString());

            _mockBatchLogGateway = new Mock<IBatchLogGateway>();
            _mockBatchLogErrorGateway = new Mock<IBatchLogErrorGateway>();
            _mockDirectDebitGateway = new Mock<IDirectDebitGateway>();
            _mockGoogleFileSettingGateway = new Mock<IGoogleFileSettingGateway>();
            _mockGoogleClientService = new Mock<IGoogleClientService>();

            _classUnderTest = new LoadDirectDebitUseCase(
                _mockBatchLogGateway.Object,
                _mockBatchLogErrorGateway.Object,
                _mockDirectDebitGateway.Object,
                _mockGoogleFileSettingGateway.Object,
                _mockGoogleClientService.Object);

            _mockBatchLogGateway
                .Setup(x => x.CreateAsync(It.IsAny<string>(), false))
                .ReturnsAsync(new BatchLogDomain { Id = 1 });
            
            var googleFileSettings = new List<GoogleFileSettingDomain>
            {
                new() { GoogleIdentifier = _sheetId }
            };

            _mockGoogleFileSettingGateway
                .Setup(x => x.GetSettingsByLabel(_directDebitLabel))
                .ReturnsAsync(googleFileSettings);
        }


        [Fact]
        public async Task ExecuteAsyncShouldLoadDirectDebitsAndReturnStepResponse()
        {
            // Arrange
            var sheetCount = _sheetNames.Length;
            var directDebits = _fixture.CreateMany<DirectDebitAuxDomain>(10).ToList();
            
            _mockGoogleClientService
                .Setup(x => x.ReadSheetToEntitiesAsync<DirectDebitAuxDomain>(_sheetId, It.IsAny<string>(), _sheetRange))
                .ReturnsAsync(directDebits);

            // Act
            var result = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Continue.Should().BeTrue();
            result.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(_waitDuration));

            _mockBatchLogGateway.Verify(x => x.CreateAsync(It.IsAny<string>(), false), Times.Once);
            _mockGoogleFileSettingGateway.Verify(x => x.GetSettingsByLabel(It.IsAny<string>()), Times.Once);
  
            _mockGoogleClientService.Verify(x => x.ReadSheetToEntitiesAsync<DirectDebitAuxDomain>(_sheetId, It.IsIn<string>("Rent", "LH"), _sheetRange),
                                            Times.Exactly(sheetCount));
            _mockDirectDebitGateway.Verify(x => x.ClearDirectDebitAuxiliary(), Times.Exactly(sheetCount));
            _mockDirectDebitGateway.Verify(x => x.CreateBulkAsync(directDebits), Times.Exactly(sheetCount));
            _mockDirectDebitGateway.Verify(x => x.LoadDirectDebit(It.IsAny<long>()), Times.Exactly(sheetCount));

            _mockBatchLogGateway.Verify(x => x.SetToSuccessAsync(It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsyncShouldContinueWhenNoDirectDebits()
        {
            // Arrange
            var sheetCount = _sheetNames.Length;
            var directDebits = new List<DirectDebitAuxDomain>();
            
            _mockGoogleClientService
                .Setup(x => x.ReadSheetToEntitiesAsync<DirectDebitAuxDomain>(_sheetId, It.IsIn(_sheetNames), _sheetRange))
                .ReturnsAsync(directDebits);
            
            // Act
            var result = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Continue.Should().BeTrue();
            result.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(_waitDuration));

            _mockBatchLogGateway.Verify(x => x.CreateAsync(It.IsAny<string>(), false), Times.Once);
            _mockGoogleFileSettingGateway.Verify(x => x.GetSettingsByLabel(It.IsAny<string>()), Times.Once);
  
            _mockGoogleClientService.Verify(x => x.ReadSheetToEntitiesAsync<DirectDebitAuxDomain>(_sheetId, It.IsIn(_sheetNames), _sheetRange),
                                            Times.Exactly(sheetCount));
            _mockDirectDebitGateway.Verify(x => x.ClearDirectDebitAuxiliary(), Times.Never);
            _mockDirectDebitGateway.Verify(x => x.CreateBulkAsync(directDebits), Times.Never);
            _mockDirectDebitGateway.Verify(x => x.LoadDirectDebit(It.IsAny<long>()), Times.Never);

            _mockBatchLogGateway.Verify(x => x.SetToSuccessAsync(It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public void ExecuteAsyncShouldCatchExceptionAndThrow()
        {
            // Arrange
            var testException = new Exception("Test Exception");

            _mockBatchLogGateway
                .Setup(x => x.CreateAsync(It.IsAny<string>(), false))
                .ThrowsAsync(testException);

            // Act + Assert
            _classUnderTest.Invoking(x => x.ExecuteAsync())
                           .Should()
                           .Throw<Exception>()
                           .WithMessage(testException.Message);
            _mockBatchLogErrorGateway.Verify(x => x.CreateAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ExecuteAsyncShouldThrowIfGoogleFileSettingsNotFound()
        {
            // Arrange
            var googleFileSettings = new List<GoogleFileSettingDomain>();
            var directDebits = _fixture.CreateMany<DirectDebitAuxDomain>(10).ToList();

            _mockGoogleFileSettingGateway
                .Setup(x => x.GetSettingsByLabel(_directDebitLabel))
                .ReturnsAsync(googleFileSettings);

            // Act + Assert
            _classUnderTest.Invoking(x => x.ExecuteAsync())
                           .Should()
                           .Throw<GoogleFileSettingNotFoundException>()
                           .WithMessage($"No Google File Settings found with label: {_directDebitLabel}.");
        }
    }
}