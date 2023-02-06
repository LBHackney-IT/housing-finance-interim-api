using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Google.Apis.Drive.v3.Data;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase;
using Moq;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class ImportCashFileUseCaseTests
    {
        private ImportCashFileUseCase _classUnderTest;
        //TODO: Make sure this works else put instantiation in setup
        private readonly Mock<IBatchLogGateway> _batchLogGateway = new Mock<IBatchLogGateway>();
        private readonly Mock<IBatchLogErrorGateway> _batchLogErrorGateway = new Mock<IBatchLogErrorGateway>();
        private readonly Mock<IGoogleFileSettingGateway> _googleFileSettingGateway = new Mock<IGoogleFileSettingGateway>();
        private readonly Mock<IGoogleClientService> _googleClientService = new Mock<IGoogleClientService>();
        private readonly Mock<IUPCashDumpFileNameGateway> _upCashDumpFileNameGateway = new Mock<IUPCashDumpFileNameGateway>();
        private readonly Mock<IUPCashDumpGateway> _upCashDumpGateway = new Mock<IUPCashDumpGateway>();

        private static Fixture _fixture = new Fixture();

        private readonly string googleIdentifier = "abc123";
        private readonly string batchId = "12345";
        private readonly long batchIdLong = 12345;
        private readonly string cashFileLabel = "CashFile";

        public ImportCashFileUseCaseTests()
        {

            _classUnderTest = new ImportCashFileUseCase(
                _batchLogGateway.Object,
                _batchLogErrorGateway.Object,
                _googleFileSettingGateway.Object,
                _googleClientService.Object,
                _upCashDumpFileNameGateway.Object,
                _upCashDumpGateway.Object
            );
        }

        private List<GoogleFileSettingDomain> CreateGoogleFileSettingDomains()
        {
            var _googleFileSettingFeature =
                _fixture.Build<GoogleFileSettingDomain>()
                    .With(domain => domain.Label, cashFileLabel)
                    .With(domain => domain.GoogleIdentifier, googleIdentifier)
                    .CreateMany().ToList();
            return _googleFileSettingFeature;
        }

        private async Task SetupGateways()
        {
            // Let googleClientService return an empty file list (no files found)
            _googleClientService.Setup(
                service => service.GetFilesInDriveAsync(googleIdentifier)
            ).ReturnsAsync(new List<File>());

            _googleFileSettingGateway.Setup(
                gateway => gateway.GetSettingsByLabel(cashFileLabel)
            ).ReturnsAsync( CreateGoogleFileSettingDomains() );

            _googleFileSettingGateway.Setup(
                gateway => gateway.GetSettingsByLabel(cashFileLabel)
            ).ReturnsAsync( CreateGoogleFileSettingDomains() );

            _batchLogGateway.Setup(
                gateway => gateway.CreateAsync(batchId, false)
            ).ReturnsAsync(new BatchLogDomain());

            _batchLogGateway.Setup(
                gateway => gateway.SetToSuccessAsync(123)
            ).ReturnsAsync(true);

            _batchLogErrorGateway.Setup(
                gateway => gateway.CreateAsync(batchIdLong, "ERROR", "message")
            ).ReturnsAsync(new BatchLogErrorDomain());
        }

        [Fact]
        public async Task ImportCashFileThrowsExceptionWhenNoFileFoundInFolder()
        {
            // Arrange
            SetupGateways();

            // _classUnderTest.ExecuteAsync();

            // Act + Assert
            _classUnderTest.Invoking(cls => cls.ExecuteAsync().ConfigureAwait(false))
                .Should().Throw<Exception>()
                .WithMessage($"No files found in folder {googleIdentifier}");
        }
    }
}
