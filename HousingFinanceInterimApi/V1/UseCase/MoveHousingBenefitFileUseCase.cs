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

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class MoveHousingBenefitFileUseCase : IMoveHousingBenefitFileUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly string _academyFileRegex = @"[0-9]{8}";
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

        public async Task<StepResponse> ExecuteAsync(string label)
        {
            LoggingHandler.LogInfo($"Checking if exist pending for {label} label");

            var batch = await _batchLogGateway.CreateAsync(_academyFileFolderLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(_academyFileFolderLabel).ConfigureAwait(false);
            var destinationGoogleFileSettings = await GetGoogleFileSetting(_housingBenefitFileLabel).ConfigureAwait(false);

            foreach (var googleFileSetting in googleFileSettings)
            {
                var folderFiles = await _googleClientService.GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier).ConfigureAwait(false);

                LoggingHandler.LogInfo($"Folder Id: {googleFileSetting.GoogleIdentifier}");
                LoggingHandler.LogInfo($"File count: {folderFiles.Count}");

                if (folderFiles.Any())
                {
                    var rg = new Regex(_academyFileRegex);
                    foreach (var file in folderFiles)
                    {
                        var matches = rg.Matches(file.Name);
                        if (matches.Count == 2)
                        {
                            var fileNameDate = DateTime.ParseExact(matches[1].Value, "ddMMyyyy", null);

                            LoggingHandler.LogInfo($"Original filename: {file.Name}");
                            var newFileName = $"HousingBenefitFile{fileNameDate.AddDays(-7).ToString("yyyyMMdd")}.dat";
                            LoggingHandler.LogInfo($"New filename: {newFileName}");

                            foreach (var destination in destinationGoogleFileSettings)
                            {
                                var filesToCheck = new string[] { newFileName, $"OK_{newFileName}", $"NOK_{newFileName}" };
                                var destinationFolderFiles = await _googleClientService.GetFilesInDriveAsync(destination.GoogleIdentifier).ConfigureAwait(false);

                                if (!destinationFolderFiles.Any(x => filesToCheck.Contains(x.Name)))
                                    await _googleClientService.CopyFileInDrive(file.Id, destination.GoogleIdentifier, newFileName).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            await _batchLogErrorGateway.CreateAsync(batch.Id, "ERROR", $"Application error. Not possible to copy academy files({file.Name})").ConfigureAwait(false);
                            LoggingHandler.LogError($"Not possible to copy academy files({file.Name})");
                        }
                    }
                }
            }

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"End academy file copy");
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
    }
}
