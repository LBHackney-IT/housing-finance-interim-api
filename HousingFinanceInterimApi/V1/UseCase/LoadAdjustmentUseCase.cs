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
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class LoadAdjustmentUseCase : ILoadAdjustmentUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IAdjustmentGateway _adjustmentGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _adjustmentLabel = "Adjustment";

        public LoadAdjustmentUseCase(
            IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IAdjustmentGateway adjustmentGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _adjustmentGateway = adjustmentGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting adjustment import");

            const string sheetName = "Active";
            const string sheetRange = "A:D";

            var batch = await _batchLogGateway.CreateAsync(_adjustmentLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(_adjustmentLabel).ConfigureAwait(false);

            if (googleFileSettings == null)
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };

            var adjustment = await _googleClientService
                .ReadSheetToEntitiesAsync<AdjustmentDomain>(googleFileSettings.GoogleIdentifier, sheetName, sheetRange)
                .ConfigureAwait(false);

            if (!adjustment.Any())
            {
                LoggingHandler.LogInfo($"No adjustment data to import");
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
            }

            await HandleSpreadSheet(batch.Id, adjustment).ConfigureAwait(false);

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"End adjustment import");
            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }

        private async Task<GoogleFileSettingDomain> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting Google file setting for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} Google file settings found");

            return googleFileSettings.FirstOrDefault();
        }

        private async Task HandleSpreadSheet(long batchId, IList<AdjustmentDomain> adjustment)
        {
            try
            {
                LoggingHandler.LogInfo($"Clear aux table");
                await _adjustmentGateway.ClearAdjustmentAuxiliary().ConfigureAwait(false);

                LoggingHandler.LogInfo($"Starting bulk insert");
                await _adjustmentGateway.CreateBulkAsync(adjustment).ConfigureAwait(false);

                LoggingHandler.LogInfo($"Starting merge adjustment");
                await _adjustmentGateway.LoadTransactions().ConfigureAwait(false);

                LoggingHandler.LogInfo("File success");
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleSpreadSheet)}";

                await _batchLogErrorGateway.CreateAsync(batchId, _adjustmentLabel, $"Application error. Not possible to load adjustment").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
