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
        private readonly IChargesBatchYearsGateway _chargesBatchYearsGateway;
        private readonly IChargesGateway _chargesGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private const string ChargesLabel = "Charges";

        public LoadChargesUseCase(
            IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IChargesBatchYearsGateway chargesBatchYearsGateway,
            IChargesGateway chargesGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _chargesBatchYearsGateway = chargesBatchYearsGateway;
            _chargesGateway = chargesGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting charges import");

            const string sheetRange = "A:AX";

            var batch = await _batchLogGateway.CreateAsync(ChargesLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(ChargesLabel).ConfigureAwait(false);

            if (!googleFileSettings.Any())
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };

            var pendingYear = await _chargesBatchYearsGateway.GetPendingYear().ConfigureAwait(false);

            foreach (var googleFile in googleFileSettings)
            {
                if (googleFile.FileYear == pendingYear.Year)
                {
                    foreach (var sheetName in Enum.GetValues(typeof(RentGroup)))
                    {
                        var chargesAux = await _googleClientService
                            .ReadSheetToEntitiesAsync<ChargesAuxDomain>(googleFile.GoogleIdentifier, sheetName.ToString(), sheetRange)
                            .ConfigureAwait(false);

                        if (!chargesAux.Any())
                        {
                            LoggingHandler.LogInfo($"No charges data to import. Sheet name: {sheetName}");
                            continue;
                        }

                        await HandleSpreadSheet(batch.Id, chargesAux, sheetName.ToString(), (int) googleFile.FileYear).ConfigureAwait(false);
                    }
                }
            }

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"End charges import");
            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }

        private async Task<List<GoogleFileSettingDomain>> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting Google file setting for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} Google file settings found");

            return googleFileSettings;
        }

        private async Task HandleSpreadSheet(long batchId, IList<ChargesAuxDomain> chargesAux, string rentGroup, int year)
        {
            try
            {
                LoggingHandler.LogInfo($"Clear aux table");
                await _chargesGateway.ClearChargesAuxiliary().ConfigureAwait(false);

                LoggingHandler.LogInfo($"Starting bulk insert");
                await _chargesGateway.CreateBulkAsync(chargesAux, rentGroup, year).ConfigureAwait(false);

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
