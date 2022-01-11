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
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class LoadChargesUseCase : ILoadChargesUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IChargesGateway _chargesGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private const string ChargesLabel = "Charges";

        public LoadChargesUseCase(
            IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IChargesGateway chargesGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _chargesGateway = chargesGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting charges import");

            var existBatchToday = await _batchLogGateway.GetAsync(ChargesLabel).ConfigureAwait(false);
            if (existBatchToday != null)
            {
                LoggingHandler.LogInfo($"Exists a charges load process today");
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(0) };
            }

            const string sheetRange = "A:AX";

            var batch = await _batchLogGateway.CreateAsync(ChargesLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(ChargesLabel).ConfigureAwait(false);

            if (googleFileSettings == null)
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };

            foreach (var sheetName in Enum.GetValues(typeof(RentGroup)))
            {
                var chargesAux = await _googleClientService
                    .ReadSheetToEntitiesAsync<ChargesAuxDomain>(googleFileSettings.GoogleIdentifier, sheetName.ToString(), sheetRange)
                    .ConfigureAwait(false);

                if (!chargesAux.Any())
                {
                    LoggingHandler.LogInfo($"No charges data to import. Sheet name: {sheetName}");
                    continue;
                }

                await HandleSpreadSheet(batch.Id, chargesAux, sheetName.ToString()).ConfigureAwait(false);
            }

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"End charges import");
            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }

        private async Task<GoogleFileSettingDomain> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting Google file setting for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} Google file settings found");

            return googleFileSettings.FirstOrDefault();
        }

        private async Task HandleSpreadSheet(long batchId, IList<ChargesAuxDomain> chargesAux, string rentGroup)
        {
            try
            {
                LoggingHandler.LogInfo($"Clear aux table");
                await _chargesGateway.ClearChargesAuxiliary().ConfigureAwait(false);

                LoggingHandler.LogInfo($"Starting bulk insert");
                await _chargesGateway.CreateBulkAsync(chargesAux, rentGroup).ConfigureAwait(false);

                LoggingHandler.LogInfo($"Starting merge charges");
                await _chargesGateway.LoadCharges().ConfigureAwait(false);

                LoggingHandler.LogInfo("File success");
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleSpreadSheet)}";

                await _batchLogErrorGateway.CreateAsync(batchId, ChargesLabel, $"Application error. Not possible to load charges").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
