using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
using FileDescription = Google.Apis.Drive.v3.Data.File;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
// needed to mock the nested class: CreateMediaUpload
using static Google.Apis.Drive.v3.FilesResource;
using Google.Apis.Upload;
using System.IO;
using System;
using FluentAssertions;
using System.Threading;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Moq.Protected;
using Newtonsoft.Json;
using Google.Apis.Http;
using Google.Apis.Services;

namespace HousingFinanceInterimApi.Tests.V1.Gateways
{
    using UploadRequesMockBehaviourOverride = Action<Mock<CreateMediaUpload>>;
    using UploadRequestInitialiser = Func<FileDescription, Stream, string, CreateMediaUpload>;

    public class GoogleClientServiceTests
    {
        private readonly IGoogleClientService _classUnderTest;
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<DriveService> _mockDriveService;
        private readonly Mock<SheetsService> _mockSheetsService;
        private readonly Mock<FilesResource> _mockFilesResource;
        private Mock<CreateMediaUpload> _mockUploadRequest; // needed for verifications
        private UploadRequestInitialiser _mockUploadRequestInitialiser;

        public GoogleClientServiceTests()
        {
            _mockLogger = new Mock<ILogger>();

            // Pass Initializer to satisfy protected constructors
            _mockDriveService = new Mock<DriveService>(new Google.Apis.Services.BaseClientService.Initializer());
            _mockSheetsService = new Mock<SheetsService>(new Google.Apis.Services.BaseClientService.Initializer());
            _mockFilesResource = new Mock<FilesResource>(_mockDriveService.Object);

            _mockDriveService
                .Setup(ds => ds.Files)
                .Returns(_mockFilesResource.Object);

            _classUnderTest = new GoogleClientService(
                _mockLogger.Object,
                _mockDriveService.Object,
                _mockSheetsService.Object
            );
        }

        [Fact]
        public async Task GoogleClientServiceUploadFileToDriveMethodTriggerUploadFlowAndDoesNotThrowUponPackageException()
        {
            // arrange
            var expectedErrorMessage = "Session refused for no reason.";
            var expectedException = new NotImplementedException(expectedErrorMessage);

            // will fail due to lack of setup
            UploadRequesMockBehaviourOverride noSetup =
                (Mock<CreateMediaUpload> _) => { };

            ConfigureUploadRequestMock(noSetup);

            _mockFilesResource
                .Setup(fr => fr.Create(
                    It.IsAny<FileDescription>(),
                    It.IsAny<Stream>(),
                    It.IsAny<string>()
                ))
                .Returns(_mockUploadRequestInitialiser);


            var fileData = new MemoryStream();
            var fileInMemory = new FileInMemory(fileData, "mName", "text/csv");
            var uploadTargetId = RandomGen.String2();

            // arrange, act
            Func<Task> uploadCallback = async () => await _classUnderTest
                .UploadFileToDrive(fileInMemory, uploadTargetId)
                .ConfigureAwait(false);

            // act, assert
            await uploadCallback.Should().NotThrowAsync().ConfigureAwait(false);

            _mockDriveService.Verify(
                ds => ds.Files,
                Times.Once
            );

            _mockFilesResource.Verify(
                fr => fr.Create(
                    It.IsAny<FileDescription>(),
                    It.IsAny<Stream>(),
                    It.IsAny<string>()
                ),
                Times.Once
            );

            _mockUploadRequest.Verify(
                ur => ur.InitiateSessionAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );

            fileData.Dispose();
        }

        [Fact]
        public async Task GoogleClientServiceUploadFileToDriveMethodReturnsTheExpectedUploadStatusAndExceptionUponInternalFailure()
        {
            // arrange
            var expectedErrorMessage = "Session refused for no reason.";
            var expectedException = new NotImplementedException(expectedErrorMessage);

            UploadRequesMockBehaviourOverride throwUponInitiatingSession =
                (Mock<CreateMediaUpload> mockUploadRequestRef) => mockUploadRequestRef
                    .Setup(r => r.InitiateSessionAsync(default))
                    .ThrowsAsync(expectedException);

            ConfigureUploadRequestMock(throwUponInitiatingSession);

            _mockFilesResource
                .Setup(fr => fr.Create(
                    It.IsAny<FileDescription>(),
                    It.IsAny<Stream>(),
                    It.IsAny<string>()
                ))
                .Returns(_mockUploadRequestInitialiser);


            var fileData = new MemoryStream();
            var fileInMemory = new FileInMemory(fileData, "mName", "text/csv");
            var uploadTargetId = RandomGen.String2();

            // act
            var uploadProgressResponse = await _classUnderTest
                .UploadFileToDrive(fileInMemory, uploadTargetId)
                .ConfigureAwait(false);

            // assert
            uploadProgressResponse.Status.Should().Be(UploadStatus.Failed);
            uploadProgressResponse.Exception.Message.Should().Be(expectedErrorMessage);
            uploadProgressResponse.Exception.Should().BeOfType<NotImplementedException>();

            fileData.Dispose();
        }

        [Fact]
        public async Task GoogleClientServiceUploadFileOrThrowMethodThrowsTheCapturedExceptionUponInternalFailure()
        {
            // arrange
            var expectedErrorMessage = "Something is malformed.";
            var expectedException = new FormatException(expectedErrorMessage);

            UploadRequesMockBehaviourOverride throwUponInitiatingSession =
                (Mock<CreateMediaUpload> mockUploadRequestRef) => mockUploadRequestRef
                    .Setup(r => r.InitiateSessionAsync(default))
                    .ThrowsAsync(expectedException);

            ConfigureUploadRequestMock(throwUponInitiatingSession);

            _mockFilesResource
                .Setup(fr => fr.Create(
                    It.IsAny<FileDescription>(),
                    It.IsAny<Stream>(),
                    It.IsAny<string>()
                ))
                .Returns(_mockUploadRequestInitialiser);


            var fileData = new MemoryStream();
            var fileInMemory = new FileInMemory(fileData, "mName", "text/csv");
            var uploadTargetId = RandomGen.String2();

            // arrange, act
            Func<Task> uploadCallback = async () => await _classUnderTest
                .UploadFileOrThrow(fileInMemory, uploadTargetId)
                .ConfigureAwait(false);

            // act, assert
            await uploadCallback
                .Should()
                .ThrowAsync<FormatException>()
                .WithMessage(expectedErrorMessage)
                .ConfigureAwait(false);

            fileData.Dispose();
        }

        [Fact]
        public async Task ReadSheetToEntitiesAsyncShouldTrimSpacesAndSkipMalformedRows()
        {
            // Arrange
            var spreadSheetId = "1234";
            var sheetName = "Sheet1";
            var range = "A1:B2";

            IList<IList<object>> values = new List<IList<object>>
            {
                new List<object> { "Prop1", "Prop2" },
                new List<object> { "  Value 1  ", "100" },
                new List<object> { "Bad Row", "NotANumber" },
                new List<object> { "Value 3", "200" }
            };

            var jsonResponse = JsonConvert.SerializeObject(new ValueRange { Values = values });

            var mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var mockHttpClientFactory = new Mock<Google.Apis.Http.IHttpClientFactory>();
            mockHttpClientFactory.Setup(f => f.CreateHttpClient(It.IsAny<CreateHttpClientArgs>()))
                .Returns(new ConfigurableHttpClient(new ConfigurableMessageHandler(mockHandler.Object)));

            var initializer = new BaseClientService.Initializer
            {
                HttpClientFactory = mockHttpClientFactory.Object,
                ApplicationName = "Test"
            };

            var sheetsService = new SheetsService(initializer);
            var driveService = new DriveService(new BaseClientService.Initializer());

            var serviceUnderTest = new GoogleClientService(_mockLogger.Object, driveService, sheetsService);

            // Act
            var results = await serviceUnderTest.ReadSheetToEntitiesAsync<TestSheetEntity>(spreadSheetId, sheetName, range).ConfigureAwait(false);

            // Assert
            results.Should().NotBeNull();
            results.Should().HaveCount(2);
            results[0].Prop1.Should().Be("Value 1");
            results[0].Prop2.Should().Be(100);
            results[1].Prop1.Should().Be("Value 3");
            results[1].Prop2.Should().Be(200);
        }

        public class TestSheetEntity
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
        }

        private void ConfigureUploadRequestMock(UploadRequesMockBehaviourOverride overrideAction)
        {
            _mockUploadRequestInitialiser = (FileDescription body, Stream stream, string contentType) =>
            {
                _mockUploadRequest = new Mock<CreateMediaUpload>(
                    _mockDriveService.Object,
                    body,
                    stream,
                    contentType
                );

                overrideAction.Invoke(_mockUploadRequest);

                return _mockUploadRequest.Object;
            };
        }
    }
}
