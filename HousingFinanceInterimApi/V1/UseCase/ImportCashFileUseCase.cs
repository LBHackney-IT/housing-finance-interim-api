using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;
using Microsoft.Extensions.Logging;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.Exceptions;
using Google.Apis.Drive.v3.Data;
using Hackney.Core.Logging;

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


        private readonly string _cashFileRegex = Environment.GetEnvironmentVariable("CASH_FILE_REGEX");
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _cashFileLabel = "CashFile";
        private readonly List<string> _listExcludedFileStartWith = new List<string>(new string[] { "OK_", "NOK_" });

        public ImportCashFileUseCase(IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService,
            IUPCashDumpFileNameGateway upCashDumpFileNameGateway,
            IUPCashDumpGateway upCashDumpGateway)

        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
            _upCashDumpFileNameGateway = upCashDumpFileNameGateway;
            _upCashDumpGateway = upCashDumpGateway;
        }

        public async virtual Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo("Starting cash file import");

            var batch = await _batchLogGateway.CreateAsync(_cashFileLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(_cashFileLabel).ConfigureAwait(false);

            foreach (var googleFileSetting in googleFileSettings)
            {

                var folderFiles = await _googleClientService.GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier).ConfigureAwait(false);

                folderFiles = folderFiles.Where(item =>
                    item.Name.EndsWith(googleFileSetting.FileType) &&
                    !_listExcludedFileStartWith.Any(y => item.Name.StartsWith(y))).ToList();

                LoggingHandler.LogInfo($"Folder Id: {googleFileSetting.GoogleIdentifier}");
                LoggingHandler.LogInfo($"File count: {folderFiles.Count}");


                if (folderFiles.Any())
                    await HandleCashFile(batch.Id, folderFiles).ConfigureAwait(false);
            }

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"End cash file import");

            return new StepResponse()
            {
                Continue = true,
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

                LoggingHandler.LogInfo($"Filename: {fileItem.Name}");
                try
                {
                    LoggingHandler.LogInfo($"Checking if filename is correct");
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

                    LoggingHandler.LogInfo($"Checking if the file has already loaded");
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

                    LoggingHandler.LogInfo($"Creating file entry");
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

                    LoggingHandler.LogInfo($"Row count: {fileLines.Count}");

                    if (fileLines.Count == 0)
                    {
                        //LoggingHandler.LogError($"No rows found in file {fileItem.Name}");
                        throw new EmptyFileException(fileItem.Name);
                    }

                    LoggingHandler.LogInfo($"Starting bulk insert");
                    await _upCashDumpGateway.CreateBulkAsync(upCashDumpFileName.Id, fileLines).ConfigureAwait(false);

                    LoggingHandler.LogInfo("File success");
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
                    throw new Exception(errorMessage);
                }
            }
        }

        private async Task LogAndRenameFileError(long batchId, string message, string messageType, File file)
        {
            if (messageType == "WARNING")
            {
                LoggingHandler.LogWarning(message);
            }
            if (messageType == "ERROR")
            {
                LoggingHandler.LogError(message);
                throw new IncorrectFileNameException(file.Id, file.Parents);
            }

            await _batchLogErrorGateway.CreateAsync(batchId, messageType, message).ConfigureAwait(false);
            await _googleClientService.RenameFileInDrive(file.Id, $"NOK_{file.Name}").ConfigureAwait(false);
        }
    }
}
