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
    public class LoadDirectDebitUseCase : ILoadDirectDebitUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IDirectDebitGateway _directDebitGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private const string DirectDebitLabel = "DirectDebit";

        public LoadDirectDebitUseCase(
            IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IDirectDebitGateway directDebitGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _directDebitGateway = directDebitGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting direct debit import");

            const string sheetNames = "Rent;LH";
            const string sheetRange = "A:C";

            var batch = await _batchLogGateway.CreateAsync(DirectDebitLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(DirectDebitLabel).ConfigureAwait(false);

            if (googleFileSettings == null)
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(0) };

            foreach (var sheetName in sheetNames.Split(";"))
            {
                var directDebits = await _googleClientService
                    .ReadSheetToEntitiesAsync<DirectDebitAuxDomain>(googleFileSettings.GoogleIdentifier, sheetName, sheetRange)
                    .ConfigureAwait(false);

                if (!directDebits.Any())
                {
                    LoggingHandler.LogInfo($"No direct debit data to import for {sheetName}");
                    continue;
                }

                await HandleSpreadSheet(batch.Id, directDebits).ConfigureAwait(false);
            }

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"End direct debit import");
            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }

        private async Task<GoogleFileSettingDomain> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting Google file setting for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} Google file settings found");

            return googleFileSettings.FirstOrDefault();
        }

        private async Task HandleSpreadSheet(long batchId, IList<DirectDebitAuxDomain> directDebits)
        {
            try
            {
                LoggingHandler.LogInfo($"Clear aux table");
                await _directDebitGateway.ClearDirectDebitAuxiliary().ConfigureAwait(false);

                LoggingHandler.LogInfo($"Starting bulk insert");
                await _directDebitGateway.CreateBulkAsync(directDebits).ConfigureAwait(false);

                LoggingHandler.LogInfo($"Starting merge direct debit");
                await _directDebitGateway.LoadDirectDebit(batchId).ConfigureAwait(false);

                LoggingHandler.LogInfo("File success");
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleSpreadSheet)}";

                await _batchLogErrorGateway.CreateAsync(batchId, "ERROR", $"Application error. Not possible to load direct debits").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
