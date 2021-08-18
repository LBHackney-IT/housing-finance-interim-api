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

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class ImportHousingFileUseCase : IImportHousingFileUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;
        private readonly IUPHousingCashDumpFileNameGateway _upHousingCashDumpFileNameGateway;
        private readonly IUPHousingCashDumpGateway _upHousingCashDumpGateway;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));
        private readonly string _housingBenefitFileRegex = Environment.GetEnvironmentVariable("HOUSING_FILE_REGEX");
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _housingBenefitFileLabel = "HousingBenefitFile";
        private readonly List<string> _listExcludedFileStartWith = new List<string>(new string[] { "OK_", "NOK_" });

        public ImportHousingFileUseCase(IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService,
            IUPHousingCashDumpFileNameGateway upHousingCashDumpFileNameGateway,
            IUPHousingCashDumpGateway upHousingCashDumpGateway)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
            _upHousingCashDumpFileNameGateway = upHousingCashDumpFileNameGateway;
            _upHousingCashDumpGateway = upHousingCashDumpGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"STARTING HOUSING BENEFIT FILE IMPORT");

            var batch = await _batchLogGateway.CreateAsync(_housingBenefitFileLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(_housingBenefitFileLabel).ConfigureAwait(false);

            foreach (var googleFileSetting in googleFileSettings)
            {
                var folderFiles = await _googleClientService.GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier).ConfigureAwait(false);

                folderFiles = folderFiles.Where(item =>
                    item.Name.EndsWith(googleFileSetting.FileType) &&
                    !_listExcludedFileStartWith.Any(y => item.Name.StartsWith(y))).ToList();

                LoggingHandler.LogInfo($"FOLDER ID: {googleFileSetting.GoogleIdentifier}");
                LoggingHandler.LogInfo($"FILE COUNT: {folderFiles.Count}");

                await HandleHousingBenefitFile(batch.Id, folderFiles).ConfigureAwait(false);
            }

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"END HOUSING BENEFIT FILE IMPORT");
            return new StepResponse()
            {
                Continue = true,
                NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };
        }

        private async Task<List<GoogleFileSettingDomain>> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"GETTING GOOGLE FILE SETTINGS FOR '{label}' LABEL");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} GOOGLE FILE SETTINGS FOUND");

            return googleFileSettings;
        }

        private static bool CheckFileName(string fileName, string regex)
        {
            Regex re = new Regex(regex);
            if (re.IsMatch(fileName))
                return true;

            return false;
        }

        private async Task HandleHousingBenefitFile(long batchId, IEnumerable<File> files)
        {
            foreach (File fileItem in files)
            {
                if (_listExcludedFileStartWith.Any(y => fileItem.Name.StartsWith(y)))
                    continue;

                LoggingHandler.LogInfo($"FILENAME: {fileItem.Name}");
                try
                {
                    LoggingHandler.LogInfo($"CHECKING IF FILENAME IS CORRECT");
                    var checkFileName = CheckFileName(fileItem.Name, _housingBenefitFileRegex);
                    if (!checkFileName)
                    {
                        await LogAndRenameFileError(batchId,
                            $"NON-STANDARD HOUSING BENEFIT FILENAME (HousingBenefitFileYYYYMMDD). CHECK FILENAME: {fileItem.Name}",
                            "WARNING",
                            fileItem)
                            .ConfigureAwait(false);

                        continue;
                    }

                    LoggingHandler.LogInfo($"CHECKING IF THE FILE HAS ALREADY LOADED");
                    var getResult = await _upHousingCashDumpFileNameGateway.GetProcessedFileByName(fileItem.Name).ConfigureAwait(false);
                    if (getResult != null)
                    {
                        await LogAndRenameFileError(batchId,
                                $"FILE {fileItem.Name} ALREADY EXIST",
                                "WARNING",
                                fileItem)
                            .ConfigureAwait(false);

                        continue;
                    }

                    LoggingHandler.LogInfo($"CREATING FILE ENTRY");
                    var upHousingashDumpFileName = await _upHousingCashDumpFileNameGateway.CreateAsync(fileItem.Name).ConfigureAwait(false);
                    if (upHousingashDumpFileName == null)
                    {
                        await LogAndRenameFileError(batchId,
                                $"FILE ENTRY {fileItem.Name} NOT CREATED",
                                "WARNING",
                                fileItem)
                            .ConfigureAwait(false);

                        continue;
                    }

                    var fileLines = await _googleClientService.ReadFileLineDataAsync(fileItem.Name, fileItem.Id, fileItem.MimeType).ConfigureAwait(false);
                    fileLines = fileLines.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();

                    LoggingHandler.LogInfo($"ROW COUNT: {fileLines.Count}");

                    var skip = 0;
                    var failure = false;
                    var batch = new List<string>();

                    do
                    {
                        batch = fileLines.Skip(skip).Take(_batchSize).ToList();
                        skip += _batchSize;

                        if (batch.Any())
                        {
                            var bulkResult = await _upHousingCashDumpGateway.CreateBulkAsync(upHousingashDumpFileName.Id, batch)
                                .ConfigureAwait(false);

                            if (bulkResult == null)
                            {
                                failure = true;

                                await LogAndRenameFileError(batchId,
                                        $"FAILURE TO LOAD ALL ROWS (FILENAME: {upHousingashDumpFileName.FileName}, ID: {upHousingashDumpFileName.Id})",
                                        "ERROR",
                                        fileItem)
                                    .ConfigureAwait(false);

                                continue;
                            }
                            LoggingHandler.LogInfo($"FILE LINES CREATED {bulkResult.Count} FOR FILE {upHousingashDumpFileName.FileName} (ID: {upHousingashDumpFileName.Id})");
                        }
                    }
                    while (batch.Any() && !failure);

                    if (!failure)
                    {
                        LoggingHandler.LogInfo("FILE SUCCESS");
                        await _googleClientService.RenameFileInDrive(fileItem.Id, $"OK_{fileItem.Name}").ConfigureAwait(false);
                        await _upHousingCashDumpFileNameGateway.SetToSuccessAsync(upHousingashDumpFileName.Id).ConfigureAwait(false);
                    }
                }
                catch (Exception exc)
                {
                    var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleHousingBenefitFile)}";

                    await _batchLogErrorGateway.CreateAsync(batchId, "ERROR", $"APPLICATION ERROR. NOT POSSIBLE TO LOAD HOUSING BENEFIT FILES ({fileItem.Name})").ConfigureAwait(false);
                    await _googleClientService.RenameFileInDrive(fileItem.Id, $"NOK_{fileItem.Name}").ConfigureAwait(false);

                    LoggingHandler.LogError($"{namespaceLabel} APPLICATION ERROR");
                    LoggingHandler.LogError(exc.ToString());

                    throw;
                }
            }
        }

        private async Task LogAndRenameFileError(long batchId, string message, string messageType, File file)
        {
            if (messageType == "WARNING")
            {
                LoggingHandler.LogWarning(message);
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
