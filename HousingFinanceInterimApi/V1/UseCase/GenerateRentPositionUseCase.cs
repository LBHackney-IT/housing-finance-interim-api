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
            LoggingHandler.LogInfo($"STARTING GENERATE RENT POSITION");

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

                    LoggingHandler.LogInfo($"FOLDER ID: {googleFileSetting.GoogleIdentifier}");
                    LoggingHandler.LogInfo($"FILE COUNT: {folderFiles.Count}");

                    LoggingHandler.LogInfo($"DELETING OLD FILES");
                    foreach (var file in folderFiles.Where(f => f.Name.Equals(fileName)).ToList())
                    {
                        await _googleClientService.DeleteFileInDrive(file.Id).ConfigureAwait(false);
                    }

                    await _googleClientService.UploadCsvFile(rentPosition, fileName, googleFileSetting.GoogleIdentifier)
                        .ConfigureAwait(false);
                }

                googleFileSettings = await GetGoogleFileSetting(_rentPositionBkpLabel).ConfigureAwait(false);
                foreach (var googleFileSetting in googleFileSettings)
                {
                    await _googleClientService.UploadCsvFile(rentPosition, $"{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                        googleFileSetting.GoogleIdentifier).ConfigureAwait(false);
                }

                await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
                LoggingHandler.LogInfo($"END GENERATE RENT POSITION");
                return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                await _batchLogErrorGateway.CreateAsync(batch.Id, _rentPositionLabel, $"APPLICATION ERROR. NOT POSSIBLE TO GENERATE RENT POSITION CSV FILE").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} APPLICATION ERROR");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }

        private async Task<List<GoogleFileSettingDomain>> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"GETTING GOOGLE FILE SETTINGS FOR '{label}' LABEL");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} GOOGLE FILE SETTINGS FOUND");

            return googleFileSettings;
        }
    }
}
