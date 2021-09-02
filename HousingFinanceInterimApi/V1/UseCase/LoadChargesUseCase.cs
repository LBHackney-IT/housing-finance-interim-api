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

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));
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
            LoggingHandler.LogInfo($"STARTING CHARGES IMPORT");

            var existBatchToday = await _batchLogGateway.GetAsync(ChargesLabel).ConfigureAwait(false);
            if (existBatchToday != null)
            {
                LoggingHandler.LogInfo($"EXISTS A DIRECT DEBIT LOAD PROCESS TODAY");
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(0) };
            }

            const string sheetName = "Active";
            const string sheetRange = "A:AZ";

            var batch = await _batchLogGateway.CreateAsync(ChargesLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(ChargesLabel).ConfigureAwait(false);

            if (googleFileSettings == null)
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };

            var chargesAux = await _googleClientService
                .ReadSheetToEntitiesAsync<ChargesAuxDomain>(googleFileSettings.GoogleIdentifier, sheetName, sheetRange)
                .ConfigureAwait(false);

            if (!chargesAux.Any())
            {
                LoggingHandler.LogInfo($"NO CHARGES DATA TO IMPORT");
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
            }

            await HandleSpreadSheet(batch.Id, chargesAux).ConfigureAwait(false);

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"END CHARGES IMPORT");
            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }

        private async Task<GoogleFileSettingDomain> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"GETTING GOOGLE FILE SETTING FOR '{label}' LABEL");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} GOOGLE FILE SETTINGS FOUND");

            return googleFileSettings.FirstOrDefault();
        }

        private async Task HandleSpreadSheet(long batchId, IList<ChargesAuxDomain> chargesAux)
        {
            try
            {
                LoggingHandler.LogInfo($"CLEAR AUX TABLE");
                await _chargesGateway.ClearChargesAuxiliary().ConfigureAwait(false);

                var skip = 0;
                var failure = false;
                List<ChargesAuxDomain> batchCharges;

                LoggingHandler.LogInfo($"STARTING BULK INSERT");

                var bulkResult = await _chargesGateway.CreateBulkAsync(chargesAux)
                    .ConfigureAwait(false);

                await _chargesGateway.LoadCharges().ConfigureAwait(false);
                LoggingHandler.LogInfo("FILE SUCCESS");
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleSpreadSheet)}";

                await _batchLogErrorGateway.CreateAsync(batchId, ChargesLabel, $"APPLICATION ERROR. NOT POSSIBLE TO LOAD CHARGES").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} APPLICATION ERROR");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
