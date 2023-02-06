using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Bogus;
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
        private readonly Mock<IBatchLogGateway> _batchLogGateway = new Mock<IBatchLogGateway>();
        private readonly Mock<IBatchLogErrorGateway> _batchLogErrorGateway = new Mock<IBatchLogErrorGateway>();
        private readonly Mock<IGoogleFileSettingGateway> _googleFileSettingGateway = new Mock<IGoogleFileSettingGateway>();
        private readonly Mock<IGoogleClientService> _googleClientService = new Mock<IGoogleClientService>();
        private readonly Mock<IUPCashDumpFileNameGateway> _upCashDumpFileNameGateway = new Mock<IUPCashDumpFileNameGateway>();
        private readonly Mock<IUPCashDumpGateway> _upCashDumpGateway = new Mock<IUPCashDumpGateway>();

        private static Fixture _fixture = new Fixture();

        private readonly string _googleIdentifier = "abc123";
        private readonly string _batchId = "12345";
        private readonly long _batchIdLong = 12345;
        private readonly string _cashFileLabel = "CashFile";
        private readonly List<string> _listExcludedFileStartWith = new List<string>(new string[] { "OK_", "NOK_" });
        private List<GoogleFileSettingDomain> _googleFileSettingDomains;
        private UPCashDumpFileNameDomain _uPCashDumpFileNameDomain;

        private string _cashFileRegex = "^CashFile\\d{4}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01]).dat$";
        private string _waitDuration = "1234567";



        public ImportCashFileUseCaseTests()
        {
            Environment.SetEnvironmentVariable("CASH_FILE_REGEX", _cashFileRegex);
            Environment.SetEnvironmentVariable("WAIT_DURATION", _waitDuration);

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
                        .With(domain => domain.Label, _cashFileLabel)
                        .With(domain => domain.GoogleIdentifier, _googleIdentifier)
                        .With(x => x.FileType, ".dat")
                        .CreateMany()
                        .ToList();
            _googleFileSettingDomains = _googleFileSettingFeature;
            return _googleFileSettingFeature;
        }

        private UPCashDumpFileNameDomain CreateCashDumpFileDomain()
        {
            _uPCashDumpFileNameDomain = _fixture.Create<UPCashDumpFileNameDomain>();
            return _uPCashDumpFileNameDomain;
        }

        private List<File> CreateFile()
        {
            var fileList = _fixture.Build<File>()
                                   .With(file => file.Name, $"CashFile20230206{_googleFileSettingDomains.First().FileType}")
                                   .CreateMany(2).ToList();

            fileList.First().Name = _listExcludedFileStartWith.First();
            return fileList;
        }

        private void SetUpGoogleClientService()
        {
            var fileList = CreateFile();
            _googleClientService.Setup(service => service.GetFilesInDriveAsync(_googleIdentifier)).ReturnsAsync(fileList);
            _googleClientService.Setup(service => service.ReadFileLineDataAsync(fileList[1].Name,
                                                                                fileList[1].Id,
                                                                                fileList[1].MimeType))
                                .Returns(_fixture.Create<Task<IList<string>>>());

            _googleClientService.Setup(service => service.RenameFileInDrive(fileList[1].Id, $"OK_{fileList[1].Name}")).ReturnsAsync(true);
        }

        private void SetupGateways()
        {
            _googleFileSettingGateway.Setup(gateway => gateway.GetSettingsByLabel(_cashFileLabel)).ReturnsAsync(CreateGoogleFileSettingDomains());

            _googleFileSettingGateway.Setup(gateway => gateway.GetSettingsByLabel(_cashFileLabel)).ReturnsAsync(CreateGoogleFileSettingDomains());

            _batchLogGateway.Setup(gateway => gateway.CreateAsync(_cashFileLabel, false)).ReturnsAsync(_fixture.Create<BatchLogDomain>());

            _batchLogGateway.Setup(gateway => gateway.SetToSuccessAsync(123)).ReturnsAsync(true);

            _batchLogErrorGateway.Setup(gateway => gateway.CreateAsync(_batchIdLong, "ERROR", "message")).ReturnsAsync(new BatchLogErrorDomain());

            _upCashDumpFileNameGateway.Setup(gateway => gateway.CreateAsync($"CashFile20230206{_googleFileSettingDomains.First().FileType}", false)).ReturnsAsync(CreateCashDumpFileDomain());

            _upCashDumpFileNameGateway.Setup(gateway => gateway.SetToSuccessAsync(_uPCashDumpFileNameDomain.Id)).ReturnsAsync(true);

            _upCashDumpGateway.Setup(gateway => gateway.CreateBulkAsync(_uPCashDumpFileNameDomain.Id, _fixture.CreateMany<string>().ToList()));
        }

        [Fact]
        public void ImportCashFileThrowsExceptionWhenNoFileFoundInFolder()
        {
            // Arrange

            // Let googleClientService return an empty file list (no files found)
            _googleClientService.Setup(service => service.GetFilesInDriveAsync(_googleIdentifier)).ReturnsAsync(new List<File>());

            SetupGateways();

            //Act + Assert
            _classUnderTest.Invoking(async cls => await cls.ExecuteAsync().ConfigureAwait(false))
               .Should().Throw<Exception>()
               .WithMessage($"[ERROR]: No files found in folder {_googleIdentifier}");
        }

        [Fact]
        public async Task ImportCashFileSuceeds()
        {
            // Arrange
            CreateGoogleFileSettingDomains();


            SetUpGoogleClientService();
            SetupGateways();

            var batchLog = _fixture.Create<BatchLogDomain>();
            _batchLogGateway.Setup(gateway => gateway.CreateAsync(_batchId, false)).ReturnsAsync(batchLog);


            // Act
            var response = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            //Assert
            response.Should().NotBeNull();
        }

        //Add a tests for non-match file name regex
        //Add tests for loaded files
        [Fact]
        public async Task RenamesIfFileNameExists()
        {
            //Arrange
            CreateGoogleFileSettingDomains();
            var fileList = CreateFile();

            SetUpGoogleClientService();
            SetupGateways();
            _upCashDumpFileNameGateway.Setup(gateway => gateway.GetProcessedFileByName(fileList.Last().Name)).Returns(_fixture.Create<Task<UPCashDumpFileNameDomain>>());


            //Act
            var response = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            //Assert
            response.Should().NotBeNull();
            //TODO: Check if it does a lambda log warning with message "File {fileItem.Name} already exist"

        }

        //Add tests for CashDumpFile == null
        [Fact]
        public async Task RenamesIfCashDumpFileIsNull()
        {
            //Arrange
            CreateGoogleFileSettingDomains();
            CreateFile();

            SetUpGoogleClientService();
            SetupGateways();
            _upCashDumpFileNameGateway.Setup(gateway => gateway.CreateAsync($"CashFile20230206{_googleFileSettingDomains.First().FileType}", false))
                                      .ReturnsAsync((UPCashDumpFileNameDomain) null);


            //Act
            var response = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            //Assert
            response.Should().NotBeNull();
            //TODO: Check if it does a lambda log warning with message "File entry {fileItem.Name} not created"
        }


    }
}
