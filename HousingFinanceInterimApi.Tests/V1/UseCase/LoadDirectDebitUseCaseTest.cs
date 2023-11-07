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

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class LoadDirectDebitUseCaseTest
    {
        private readonly Mock<IBatchLogGateway> _mockBatchLogGateway;
        private readonly Mock<IBatchLogErrorGateway> _mockBatchLogErrorGateway;
        private readonly Mock<IDirectDebitGateway> _mockDirectDebitGateway;
        private readonly Mock<IGoogleFileSettingGateway> _mockGoogleFileSettingGateway;
        private readonly Mock<IGoogleClientService> _mockGoogleClientService;

        private readonly ILoadDirectDebitUseCase _classUnderTest;
        private readonly double _waitDuration = 30;
        private readonly string _directDebitLabel = "DirectDebit";
        private readonly string _sheetId = "google_identifier";

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
                new() {
                }
            };

            _mockGoogleFileSettingGateway
                .Setup(x => x.GetSettingsByLabel(_directDebitLabel))
                .ReturnsAsync(googleFileSettings);
        }

        [Fact]
        public async Task ExecuteAsyncShouldLoadDirectDebitsAndReturnStepResponse()
        {
            // Arrange
            const string sheetNames = "Rent;LH";
            const string sheetRange = "A:C";

            var sheetCount = sheetNames.Split(";").Length;
            var directDebits = new List<DirectDebitAuxDomain>();
            
            _mockGoogleClientService
                .Setup(x => x.ReadSheetToEntitiesAsync<DirectDebitAuxDomain>(_sheetId, It.IsAny<string>(), sheetRange))
                .ReturnsAsync(directDebits);

            // Act
            var result = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Continue.Should().BeTrue();
            result.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(_waitDuration));

            _mockBatchLogGateway.Verify(x => x.CreateAsync(It.IsAny<string>(), false), Times.Once);
            _mockGoogleFileSettingGateway.Verify(x => x.GetSettingsByLabel(It.IsAny<string>()), Times.Once);

            _mockGoogleClientService.Verify(x => x.ReadSheetToEntitiesAsync<DirectDebitAuxDomain>(_sheetId, It.IsIn<string>("Rent", "LH"), sheetRange),
                                            Times.Exactly(sheetCount));
            _mockDirectDebitGateway.Verify(x => x.ClearDirectDebitAuxiliary(), Times.Exactly(sheetCount));
            _mockDirectDebitGateway.Verify(x => x.CreateBulkAsync(directDebits), Times.Exactly(sheetCount));
            _mockDirectDebitGateway.Verify(x => x.LoadDirectDebit(It.IsAny<long>()), Times.Exactly(sheetCount));
            _mockBatchLogGateway.Verify(x => x.SetToSuccessAsync(It.IsAny<long>()), Times.Once);
        }

        // [Fact]
        // public void ExecuteAsyncShouldCatchExceptionAndThrow()
        // {
        //     // Arrange
        //     var testException = new Exception("Test Exception");

        //     _mockGoogleFileSettingGateway
        //         .Setup(x => x.GetSettingsByLabel(It.IsAny<string>()))
        //         .ThrowsAsync(testException);

        //     // Act + Assert
        //     _classUnderTest.Invoking(x => x.ExecuteAsync())
        //                    .Should()
        //                    .Throw<GoogleFileSettingNotFoundException>()
        //                    .WithMessage($"Google file setting not found for '{LoadDirectDebitUseCase.DirectDebitLabel}' label");
        //     _mockBatchLogErrorGateway.Verify(x => x.CreateAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        // }

        // [Fact]
        // public void ExecuteAsyncShouldCatchExceptionAndThrowWhenNoDirectDebits()
        // {
        //     // Arrange
        //     const string sheetNames = "Rent;LH";
        //     const string sheetRange = "A:C";
        //     var googleFileSettings = new List<GoogleFileSettingDomain>
        //     {
        //         new GoogleFileSettingDomain
        //         {
        //             GoogleIdentifier = "google_identifier"
        //         }
        //     };
        //     var directDebits = new List<DirectDebitAuxDomain>();

        //     _mockGoogleFileSettingGateway
        //         .Setup(x => x.GetSettingsByLabel(It.IsAny<string>()))
        //         .ReturnsAsync(googleFileSettings);

        //     _mockGoogleClientService
        //         .Setup(x => x.ReadSheetToEntitiesAsync<DirectDebitAuxDomain>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //         .ReturnsAsync(directDebits);

        //     // Act + Assert
        //     _classUnderTest.Invoking(x => x.ExecuteAsync())
        //                    .Should()
        //                    .NotThrow();
        //     _mockBatchLogGateway.Verify(x => x.CreateAsync(It.IsAny<string>()), Times.Once);
        //     _mockGoogleFileSettingGateway.Verify(x => x.GetSettingsByLabel(It.IsAny<string>()), Times.Once);
        //     _mockGoogleClientService.Verify(x => x.ReadSheetToEntitiesAsync<DirectDebitAuxDomain>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(sheetNames.Split(";").Length));
        //     _mockDirectDebitGateway.Verify(x => x.ClearDirectDebitAuxiliary(), Times.Never);
        //     _mockDirectDebitGateway.Verify(x => x.CreateBulkAsync(directDebits), Times.Never);
        //     _mockDirectDebitGateway.Verify(x => x.LoadDirectDebit(It.IsAny<long>()), Times.Never);
        //     _mockBatchLogGateway.Verify(x => x.SetToSuccessAsync(It.IsAny<long>()), Times.Once);
        // }
    }
}