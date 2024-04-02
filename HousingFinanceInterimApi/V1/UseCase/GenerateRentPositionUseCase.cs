using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using Google.Apis.Drive.v3.Data;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class GenerateRentPositionUseCase : IGenerateRentPositionUseCase
    {
        private readonly IRentPositionGateway _rentPositionGateway;
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _rentPositionLabel = "RentPosition";
        private readonly string _rentPositionBkpLabel = "RentPositionBkp";

        public GenerateRentPositionUseCase(IRentPositionGateway rentPositionGateway,
            IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService)
        {
            _rentPositionGateway = rentPositionGateway;
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting generate rent position");

            var fileName = "RentPosition.csv";
            var batch = await _batchLogGateway.CreateAsync(_rentPositionLabel).ConfigureAwait(false);

            try
            {
                var googleFileSettings = await GetGoogleFileSetting(_rentPositionLabel).ConfigureAwait(false);
                if (googleFileSettings == null)
                    return new StepResponse()
                    {
                        Continue = false,
                        NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                    };

                var rentPosition = await _rentPositionGateway.GetRentPosition().ConfigureAwait(false);
                if (!rentPosition.Any())
                    return new StepResponse()
                    {
                        Continue = false,
                        NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                    };

                foreach (var googleFileSetting in googleFileSettings)
                {
                    var folderFiles = await _googleClientService.GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier)
                        .ConfigureAwait(false);

                    LoggingHandler.LogInfo($"Folder ID: {googleFileSetting.GoogleIdentifier}");
                    LoggingHandler.LogInfo($"File count: {folderFiles.Count}");

                    LoggingHandler.LogInfo($"Deleting old files");
                    foreach (var file in folderFiles.Where(f => f.Name.Equals(fileName)).ToList())
                    {
                        await _googleClientService.DeleteFileInDrive(file.Id).ConfigureAwait(false);
                    }

                    var isSuccess = await _googleClientService.UploadCsvFile(rentPosition, fileName, googleFileSetting.GoogleIdentifier)
                        .ConfigureAwait(false);

                    if (!isSuccess)
                        throw new Exception("Failed to upload to Rent Position folder (Qlik)");

                }

                googleFileSettings = await GetGoogleFileSetting(_rentPositionBkpLabel).ConfigureAwait(false);
                foreach (var googleFileSetting in googleFileSettings)
                {
                    var isSuccess = await _googleClientService.UploadCsvFile(rentPosition, $"{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                        googleFileSetting.GoogleIdentifier).ConfigureAwait(false);
                    var fileQueryFields = "nextPageToken, files(id, name, createdTime)";
                    var folderFiles = await _googleClientService.GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier, fileQueryFields)
                        .ConfigureAwait(false);

                    if (!isSuccess)
                        throw new Exception("Failed to upload to Rent Position folder (Backup)");

                    var filesCreatedInLast7Days = folderFiles.Where(file =>
                        file.CreatedTime.HasValue
                        && file.CreatedTime?.Date > DateTime.Today.AddDays(-7).Date).ToList();
                    var lastFilesForFinancialYears = getLastFilesForFinancialYears(folderFiles);

                    var filesToDelete = folderFiles.Where(file =>
                            file.CreatedTime.HasValue
                            && !filesCreatedInLast7Days.Contains(file)
                            && !lastFilesForFinancialYears.Contains(file)
                        ).ToList();

                    string fileSummary(File file) => $"{file.Name} Created:({file.CreatedTime?.Date:dd/MM/yyyy})";
                    LoggingHandler.LogInfo($"All files: [{string.Join(", ", folderFiles.Select(fileSummary))}]");
                    LoggingHandler.LogInfo($"Preserving last files for past financial years: [{string.Join(", ", lastFilesForFinancialYears.Select(fileSummary))}]");
                    LoggingHandler.LogInfo($"Preserving files created in the last 7 days: [{string.Join(", ", filesCreatedInLast7Days.Select(fileSummary))}]");
                    LoggingHandler.LogInfo($"Will delete {filesToDelete.Count} backup file(s) from {googleFileSetting.GoogleIdentifier}: [{string.Join(", ", filesToDelete.Select(f => f.Name))}]");

                    var deletionErrors = new List<Exception>();
                    foreach (var file in filesToDelete)
                    {
                        try
                        {
                            LoggingHandler.LogInfo($"Deleting file {file.Name}, createdTime: {file.CreatedTime}");
                            await _googleClientService.DeleteFileInDrive(file.Id).ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            LoggingHandler.LogInfo($"Could not delete file {file.Name}");
                            deletionErrors.Add(e);
                        }
                    }
                    if (deletionErrors.Any())
                    {
                        throw new AggregateException("Could not delete all files", deletionErrors);
                    }
                }

                await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
                LoggingHandler.LogInfo($"End generate rent position");
                return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                await _batchLogErrorGateway.CreateAsync(batch?.Id, _rentPositionLabel, $"Application error. Not possible to generate rent position csv file").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }

        private async Task<List<GoogleFileSettingDomain>> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting Google file settings for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} google file settings found");

            return googleFileSettings;
        }

        /// <summary>
        /// Filters out each file that is the last file for a financial year (31st March)
        /// </summary
        static IEnumerable<File> getLastFilesForFinancialYears(IEnumerable<File> fileList)
        {
            // Get list of file groups on the last working in March for each year
            var marchFileGroupsNotOnWeekend = fileList
                .Where(f => f.CreatedTime.HasValue)
                .OrderBy(f => f.CreatedTime)
                .Where(f => !new[] { DayOfWeek.Saturday, DayOfWeek.Sunday }.Contains(f.CreatedTime.Value.DayOfWeek))
                .Where(f => f.CreatedTime.Value.Month == 3)
                .GroupBy(f => f.CreatedTime.Value.Year)
                .ToList();

            var filesOnLastDayOfFinanacialYear = new List<File>();
            foreach (var marchList in marchFileGroupsNotOnWeekend)
            {
                filesOnLastDayOfFinanacialYear.Add(marchList.Last());
            }

            return filesOnLastDayOfFinanacialYear;
        }
    }
}
