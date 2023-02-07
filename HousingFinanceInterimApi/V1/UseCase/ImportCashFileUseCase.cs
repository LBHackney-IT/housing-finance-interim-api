using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using Google.Apis.Drive.v3.Data;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class ImportCashFileUseCase : IImportCashFileUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;
        private readonly IUPCashDumpFileNameGateway _upCashDumpFileNameGateway;
        private readonly IUPCashDumpGateway _upCashDumpGateway;
        private readonly ILogger<ImportCashFileUseCase> _logger;

        private readonly string _cashFileRegex = Environment.GetEnvironmentVariable("CASH_FILE_REGEX");
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _cashFileLabel = "CashFile";
        private readonly List<string> _listExcludedFileStartWith = new List<string>(new string[] { "OK_", "NOK_" });

        public ImportCashFileUseCase(IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService,
            IUPCashDumpFileNameGateway upCashDumpFileNameGateway,
            IUPCashDumpGateway upCashDumpGateway,
            ILogger<ImportCashFileUseCase> logger)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
            _upCashDumpFileNameGateway = upCashDumpFileNameGateway;
            _upCashDumpGateway = upCashDumpGateway;
            _logger = logger;
        }

        public async Task<StepResponse> ExecuteAsync()
        {

            //_logger.LogInformation("Starting cash file import");

            var batch = await _batchLogGateway.CreateAsync(_cashFileLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(_cashFileLabel).ConfigureAwait(false);

            foreach (var googleFileSetting in googleFileSettings)
            {

                var folderFiles = await _googleClientService.GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier).ConfigureAwait(false);

                folderFiles = folderFiles.Where(item =>
                    item.Name.EndsWith(googleFileSetting.FileType) &&
                    !_listExcludedFileStartWith.Any(y => item.Name.StartsWith(y))).ToList();

                _logger.LogInformation($"Folder Id: {googleFileSetting.GoogleIdentifier}");
                _logger.LogInformation($"File count: {folderFiles.Count}");

                if (folderFiles.Count == 0)
                {
                    LoggingHandler.LogError($"No files found in folder {googleFileSetting.GoogleIdentifier}");
                }


                if (folderFiles.Any())
                    await HandleCashFile(batch.Id, folderFiles).ConfigureAwait(false);
            }

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            _logger.LogInformation($"End cash file import");

            return new StepResponse()
            {
                Continue = true,
                NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };
        }

        private async Task<List<GoogleFileSettingDomain>> GetGoogleFileSetting(string label)
        {
            _logger.LogInformation($"Getting google file settings for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            _logger.LogInformation($"{googleFileSettings.Count} Google file settings found");

            return googleFileSettings;
        }

        private static bool CheckFileName(string fileName, string regex)
        {
            Regex re = new Regex(regex);
            if (re.IsMatch(fileName))
                return true;

            return false;
        }

        private async Task HandleCashFile(long batchId, IEnumerable<File> files)
        {
            foreach (File fileItem in files)
            {
                if (_listExcludedFileStartWith.Any(y => fileItem.Name.StartsWith(y)))
                    continue;

                _logger.LogInformation($"Filename: {fileItem.Name}");
                try
                {
                    _logger.LogInformation($"Checking if filename is correct");
                    var checkFileName = CheckFileName(fileItem.Name, _cashFileRegex);
                    if (!checkFileName)
                    {
                        await LogAndRenameFileError(batchId,
                            $"Non-standard cash filename (CashFileYYYYMMDD). Check file id: {fileItem.Id} in folder(s) {fileItem.Parents}",
                            "ERROR",
                            fileItem)
                            .ConfigureAwait(false);

                        continue;
                    }

                    _logger.LogInformation($"Checking if the file has already loaded");
                    var getResult = await _upCashDumpFileNameGateway.GetProcessedFileByName(fileItem.Name).ConfigureAwait(false);
                    if (getResult != null)
                    {
                        await LogAndRenameFileError(batchId,
                                $"File {fileItem.Name} already exist",
                                "WARNING",
                                fileItem)
                            .ConfigureAwait(false);

                        continue;
                    }

                    _logger.LogInformation($"Creating file entry");
                    var upCashDumpFileName = await _upCashDumpFileNameGateway.CreateAsync(fileItem.Name).ConfigureAwait(false);
                    if (upCashDumpFileName == null)
                    {
                        await LogAndRenameFileError(batchId,
                                $"File entry {fileItem.Name} not created",
                                "WARNING",
                                fileItem)
                            .ConfigureAwait(false);

                        continue;
                    }

                    var fileLines = await _googleClientService.ReadFileLineDataAsync(fileItem.Name, fileItem.Id, fileItem.MimeType).ConfigureAwait(false);
                    fileLines = fileLines.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();

                    _logger.LogInformation($"Row count: {fileLines.Count}");

                    if (fileLines.Count == 0)
                    {
                        LoggingHandler.LogError($"No rows found in file {fileItem.Name}");
                    }

                    _logger.LogInformation($"Starting bulk insert");
                    await _upCashDumpGateway.CreateBulkAsync(upCashDumpFileName.Id, fileLines).ConfigureAwait(false);

                    _logger.LogInformation("File success");
                    await _googleClientService.RenameFileInDrive(fileItem.Id, $"OK_{fileItem.Name}").ConfigureAwait(false);
                    await _upCashDumpFileNameGateway.SetToSuccessAsync(upCashDumpFileName.Id).ConfigureAwait(false);
                }
                catch (Exception exc)
                {
                    var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleCashFile)}";

                    var errorMessage = $"Application error. Not possible to load cash files for ({fileItem.Name})\nReason: {exc.Message.ToString()}";

                    await _batchLogErrorGateway.CreateAsync(batchId, "ERROR", errorMessage).ConfigureAwait(false);
                    await _googleClientService.RenameFileInDrive(fileItem.Id, $"NOK_{fileItem.Name}").ConfigureAwait(false);

                    LoggingHandler.LogError($"{namespaceLabel} {errorMessage}");
                    LoggingHandler.LogError(exc.ToString());

                    throw;
                }
            }
        }

        private async Task LogAndRenameFileError(long batchId, string message, string messageType, File file)
        {
            if (messageType == "WARNING")
            {
                _logger.LogWarning(message);
                //LoggingHandler.LogWarning(message);
            }
            else
            {
                LoggingHandler.LogError(message);
            }

            await _batchLogErrorGateway.CreateAsync(batchId, messageType, message).ConfigureAwait(false);
            await _googleClientService.RenameFileInDrive(file.Id, $"NOK_{file.Name}").ConfigureAwait(false);
        }
    }
}
