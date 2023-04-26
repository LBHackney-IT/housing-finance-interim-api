using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;
using System.Text.RegularExpressions;
using Google.Apis.Drive.v3.Data;
using SIO = System.IO;
using Google;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class MoveHousingBenefitFileUseCase : IMoveHousingBenefitFileUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly Regex _academyFilePattern = new Regex(@"[0-9]{8}");
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _academyFileFolderLabel = "AcademyFileFolder";
        private readonly string _housingBenefitFileLabel = "HousingBenefitFile";

        public MoveHousingBenefitFileUseCase(IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            BatchLogDomain batch = null;

            try
            {
                LoggingHandler.LogInfo($"Checking if exist pending for {_housingBenefitFileLabel} label");

                // Register the start of the process & get a 'batch id' to log other errors against.
                batch = await _batchLogGateway.CreateAsync(_academyFileFolderLabel).ConfigureAwait(false);

                // Retrieve Academy (data source) and Housing Beneft (destination) folder Google identifier ids.
                var academyFoldersSettings = await GetGoogleFileSetting(_academyFileFolderLabel).ConfigureAwait(false);
                var destinationGoogleFileSettings = await GetGoogleFileSetting(_housingBenefitFileLabel).ConfigureAwait(false);

                if (!academyFoldersSettings.Any())
                    throw new Exception($"No file settings with label: '{_academyFileFolderLabel}' were found.");

                if (!destinationGoogleFileSettings.Any())
                    throw new Exception($"No file settings with label: '{_housingBenefitFileLabel}' were found.");

                // Retrieve all Academy files that potentially need to be copied.
                var academyFiles = new List<File>();
                foreach (var academyFolderSetting in academyFoldersSettings)
                {
                    var folderFiles = await _googleClientService.GetFilesInDriveAsync(academyFolderSetting.GoogleIdentifier).ConfigureAwait(false);

                    // I believe, this should be logged within the Gateway method
                    LoggingHandler.LogInfo($"Folder Id: {academyFolderSetting.GoogleIdentifier}");
                    LoggingHandler.LogInfo($"File count: {folderFiles.Count}");

                    academyFiles.AddRange(folderFiles);
                }

                if (!academyFiles.Any())
                    throw new SIO.FileNotFoundException($"No files were found within the '{_academyFileFolderLabel}' label directories.");

                Predicate<File> isValidFileName = (file) => _academyFilePattern.Matches(file.Name).Count == 2;

                var validAcademyFiles = academyFiles.Where(f => isValidFileName(f)).ToList();
                var notValidAcademyFiles = academyFiles.Where(f => !isValidFileName(f)).ToList();

                foreach (var file in notValidAcademyFiles)
                {
                    var errorMessage = $"Not possible to copy academy files({file.Name})";
                    LoggingHandler.LogError(errorMessage);
                    await _batchLogErrorGateway.CreateAsync(batch.Id, "ERROR", $"Application error. {errorMessage}").ConfigureAwait(false);
                };

                if (!validAcademyFiles.Any())
                    throw new SIO.FileNotFoundException($"No files with valid name were found within the '{_academyFileFolderLabel}' label directories.");

                var validRenamedAcademyFiles = validAcademyFiles
                    .Select(file => new
                    {
                        Id = file.Id,
                        OldName = file.Name,
                        NewName = CalculateNewFileName(file)
                    })
                    .ToList();

                foreach (var file in validRenamedAcademyFiles)
                {
                    // Throw exception if there are multiple files in the same week
                    var fileNames = validRenamedAcademyFiles.Select(f => f.NewName).ToList();
                    var duplicateFileNames = fileNames.GroupBy(x => x)
                        .Where(g => g.Count() > 1)
                        .Select(y => y.Key)
                        .ToList();
                    var duplicateFiles = validRenamedAcademyFiles.Where(f => duplicateFileNames.Contains(f.NewName)).ToList();
                    foreach (var duplicateFile in duplicateFiles)
                    {
                        var errorMessage = $"{duplicateFile.NewName} from {duplicateFile.OldName} is a duplicate file";
                        LoggingHandler.LogError(errorMessage);
                        await _batchLogErrorGateway.CreateAsync(batch.Id, "ERROR", $"Application error. {errorMessage}").ConfigureAwait(false);
                    }
                    if (duplicateFileNames.Any())
                    {
                        throw new Exception($"Multiple files in week with name [{string.Join(", ", duplicateFileNames)}] found");
                    }
                }

                // Create a folder with files object - makes further validation easier.
                var destinationFolderWithFiles = await Task.WhenAll(
                    destinationGoogleFileSettings.Select(async setting =>
                        new
                        {
                            Id = setting.GoogleIdentifier,
                            Files = await _googleClientService.GetFilesInDriveAsync(setting.GoogleIdentifier).ConfigureAwait(false)
                        })
                        .ToList()
                    );

                var copyFileInstructions = destinationFolderWithFiles
                    .SelectMany(destinationFolder => validRenamedAcademyFiles
                        // Avoid duplicating existing files at destination folder
                        .Where(academyFile =>
                        {
                            var academyNewNamePattern = new Regex($"(?>N?OK_)?(?>{academyFile.NewName})");
                            var doesAcademyFileExistAmongDestinationFiles =
                                destinationFolder.Files.Any(destFile => academyNewNamePattern.IsMatch(destFile.Name));
                            return !doesAcademyFileExistAmongDestinationFiles;
                        })
                        // Create object with information needed for 'copy' action
                        .Select(academyFile => new
                        {
                            FileGId = academyFile.Id,
                            FileName = academyFile.NewName,
                            DestinationFolderGId = destinationFolder.Id
                        })
                    );

                foreach (var copyInstruction in copyFileInstructions)
                {
                    await _googleClientService
                        .CopyFileInDrive(copyInstruction.FileGId, copyInstruction.DestinationFolderGId, copyInstruction.FileName)
                        .ConfigureAwait(false);
                }

                await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);

                LoggingHandler.LogInfo($"End academy file copy");

                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (GoogleApiException ex)
            {
                LoggingHandler.LogError(ex.Message);
                await _batchLogErrorGateway.CreateAsync(batch.Id, "ERROR", ex.Message);
                throw;
            }
            catch (SIO.FileNotFoundException ex)
            {
                LoggingHandler.LogError(ex.Message);
                await _batchLogErrorGateway.CreateAsync(batch.Id, "ERROR", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError(ex.Message);
                await _batchLogErrorGateway.CreateAsync(batch.Id, "ERROR", ex.Message);
                throw;
            }
        }

        private async Task<List<GoogleFileSettingDomain>> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting google file settings for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} Google file settings found");

            return googleFileSettings;
        }

        private static string CalculateNewFileName(File file)
        {
            // Handle null creation date - this should never happen
            if (file.CreatedTime == null)
            {
                var errorMsg = $"File {file.Name} in folder(s) {String.Join(", ", file.Parents)} has no creation date";
                LoggingHandler.LogError(errorMsg);
                throw new Exception(errorMsg);
            }

            var createdTime = file.CreatedTime.Value;
            var nextMondayDate = GetFollowingMondayDate(createdTime);

            var newFileName = $"HousingBenefitFile{nextMondayDate}.dat";
            LoggingHandler.LogInfo($"File {file.Name} in folder(s) {String.Join(", ", file.Parents)} will be renamed to {newFileName}");
            return newFileName;
        }

        private static string GetFollowingMondayDate(DateTime fileCreatedDate)
        {
            // Get string with formatted date of next Monday after file creation time
            // DayOfWeek ranges from 0 (Sunday) - 6 (Saturday)
            var daysUntilNextMonday = ((int) DayOfWeek.Monday - (int) fileCreatedDate.DayOfWeek + 7) % 7;
            var nextMonday = fileCreatedDate.AddDays(daysUntilNextMonday);
            return nextMonday.ToString("yyyyMMdd");
        }
    }
}
