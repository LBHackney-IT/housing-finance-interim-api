using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using Google.Apis.Drive.v3.Data;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Text.RegularExpressions;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class CheckExistFileUseCase : ICheckExistFileUseCase
    {
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private readonly string _bucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
        private readonly string _prefix = Environment.GetEnvironmentVariable("S3_OBJECT_PREFIX");

        private readonly List<string> _listExcludedFileStartWith = new List<string>(new string[] { "OK_", "NOK_" });

        public CheckExistFileUseCase(IGoogleFileSettingGateway googleFileSettingGateway, IGoogleClientService googleClientService)
        {
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync(string label)
        {
            LoggingHandler.LogInfo($"Checking if exist pending for {label} label (S3 Bucket)");
            var existFile = false;

            var googleFileSettings = await GetGoogleFileSetting(label).ConfigureAwait(false);

            var s3Client = new AmazonS3Client();
            var requestObject = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = _prefix,
                // Use this to exclude the directory name
                StartAfter = _prefix
            };

            LoggingHandler.LogInfo($"Getting file(s) from S3 Bucket");
            var responseObjects = await s3Client.ListObjectsV2Async(requestObject).ConfigureAwait(false);
            if (responseObjects.S3Objects.Any())
            {
                LoggingHandler.LogInfo($"{responseObjects.S3Objects.Count} objects found!");
                foreach (S3Object s3Object in responseObjects.S3Objects)
                {
                    LoggingHandler.LogInfo($"Working on {s3Object.Key}");
                    var file = await s3Client.GetObjectAsync(new GetObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = s3Object.Key
                    }).ConfigureAwait(false);

                    var fileDate = Regex.Match(s3Object.Key, @"(\d{2}\d{2}\d{1,4})").Value;
                    var newFilename = $"CashFile{fileDate.Substring(4, 4)}{fileDate.Substring(2, 2)}{fileDate.Substring(0, 2)}.dat";

                    LoggingHandler.LogInfo($"Saving file on Google Drive");
                    MemoryStream memoryStream = new MemoryStream();
                    using (Stream responseStream = file.ResponseStream)
                    {
                        responseStream.CopyTo(memoryStream);
                        foreach (var googleFileSetting in googleFileSettings)
                        {
                            await _googleClientService
                                .UploadFileInDrive(memoryStream, fileDate, googleFileSetting.GoogleIdentifier)
                                .ConfigureAwait(false);
                        }

                    }


                    var destinationKey = $"{s3Object.Key.Split("/")[0]}/OK_{s3Object.Key.Split("/")[1]}";
                    LoggingHandler.LogInfo($"Creating new file {destinationKey} on S3 Bucket");
                    CopyObjectRequest requestCopy = new CopyObjectRequest
                    {
                        SourceBucket = _bucketName,
                        SourceKey = s3Object.Key,
                        DestinationBucket = _bucketName,
                        DestinationKey = destinationKey
                    };
                    CopyObjectResponse responseCopy = await s3Client.CopyObjectAsync(requestCopy).ConfigureAwait(false);

                    LoggingHandler.LogInfo($"Deleting file {s3Object.Key} on S3 Bucket");
                    var requestDelete = new DeleteObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = s3Object.Key
                    };
                    var responseDelete = await s3Client.DeleteObjectAsync(requestDelete).ConfigureAwait(false);
                }

                existFile = true;
            }

            LoggingHandler.LogInfo($"Exist pending for {label} label: {existFile}");
            return new StepResponse()
            {
                Continue = existFile,
                NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };
        }

        private async Task<List<GoogleFileSettingDomain>> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting google file settings for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} Google file settings found");

            return googleFileSettings;
        }
    }
}
