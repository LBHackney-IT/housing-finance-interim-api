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
            _mockDriveService = new Mock<DriveService>();
            _mockSheetsService = new Mock<SheetsService>();
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
