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
    public class CheckExistFileUseCase : ICheckExistFileUseCase
    {
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly List<string> _listExcludedFileStartWith = new List<string>(new string[] { "OK_", "NOK_" });

        public CheckExistFileUseCase(IGoogleFileSettingGateway googleFileSettingGateway, IGoogleClientService googleClientService)
        {
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync(string label)
        {
            LoggingHandler.LogInfo($"CHECKING IF EXIST PENDING FOR {label} LABEL");
            var existFile = false;
            var googleFileSettings = await GetGoogleFileSetting(label).ConfigureAwait(false);

            foreach (var googleFileSetting in googleFileSettings)
            {
                var folderFiles = await _googleClientService.GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier).ConfigureAwait(false);

                folderFiles = folderFiles.Where(item =>
                    item.Name.EndsWith(googleFileSetting.FileType) &&
                    !_listExcludedFileStartWith.Any(y => item.Name.StartsWith(y))).ToList();

                if (folderFiles.Any())
                {
                    existFile = true;
                    break;
                }
            }

            LoggingHandler.LogInfo($"EXIST PENDING FOR {label} LABEL: {existFile}");
            return new StepResponse()
            {
                Continue = existFile,
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
    }
}
