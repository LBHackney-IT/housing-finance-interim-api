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
            LoggingHandler.LogInfo($"Starting housing benefit file import");

            var batch = await _batchLogGateway.CreateAsync(_housingBenefitFileLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(_housingBenefitFileLabel).ConfigureAwait(false);

            foreach (var googleFileSetting in googleFileSettings)
            {
                var folderFiles = await _googleClientService.GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier).ConfigureAwait(false);

                folderFiles = folderFiles.Where(item =>
                    item.Name.EndsWith(googleFileSetting.FileType) &&
                    !_listExcludedFileStartWith.Any(y => item.Name.StartsWith(y))).ToList();

                LoggingHandler.LogInfo($"Folder id: {googleFileSetting.GoogleIdentifier}");
                LoggingHandler.LogInfo($"File count: {folderFiles.Count}");

                await HandleHousingBenefitFile(batch.Id, folderFiles).ConfigureAwait(false);
            }

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"End housing benefit file import");
            return new StepResponse()
            {
                Continue = true,
                NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };
        }

        private async Task<List<GoogleFileSettingDomain>> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting Google file settings for '{label}' label");
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

        private async Task HandleHousingBenefitFile(long batchId, IEnumerable<File> files)
        {
            foreach (File fileItem in files)
            {
                if (_listExcludedFileStartWith.Any(y => fileItem.Name.StartsWith(y)))
                    continue;

                LoggingHandler.LogInfo($"Filename: {fileItem.Name}");
                try
                {
                    LoggingHandler.LogInfo($"Checking if filename is correct");
                    var checkFileName = CheckFileName(fileItem.Name, _housingBenefitFileRegex);
                    if (!checkFileName)
                    {
                        await LogAndRenameFileError(batchId,
                            $"Non-standard housing benefit filename (HousingBenefitFileYYYYMMDD). Check filename: {fileItem.Name}",
                            "WARNING",
                            fileItem)
                            .ConfigureAwait(false);

                        continue;
                    }

                    LoggingHandler.LogInfo($"Checking if the file has already loaded");
                    var getResult = await _upHousingCashDumpFileNameGateway.GetProcessedFileByName(fileItem.Name).ConfigureAwait(false);
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
                    var upHousingashDumpFileName = await _upHousingCashDumpFileNameGateway.CreateAsync(fileItem.Name).ConfigureAwait(false);
                    if (upHousingashDumpFileName == null)
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

                    LoggingHandler.LogInfo($"Starting bulk insert");
                    await _upHousingCashDumpGateway.CreateBulkAsync(upHousingashDumpFileName.Id, fileLines).ConfigureAwait(false);

                    LoggingHandler.LogInfo("File success");

                    await _googleClientService.RenameFileInDrive(fileItem.Id, $"OK_{fileItem.Name}").ConfigureAwait(false);
                    await _upHousingCashDumpFileNameGateway.SetToSuccessAsync(upHousingashDumpFileName.Id).ConfigureAwait(false);

                }
                catch (Exception exc)
                {
                    var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleHousingBenefitFile)}";

                    await _batchLogErrorGateway.CreateAsync(batchId, "ERROR", $"Application error. Not possible to load housing benefit files({fileItem.Name})").ConfigureAwait(false);
                    await _googleClientService.RenameFileInDrive(fileItem.Id, $"NOK_{fileItem.Name}").ConfigureAwait(false);

                    LoggingHandler.LogError($"{namespaceLabel} Application error");
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
