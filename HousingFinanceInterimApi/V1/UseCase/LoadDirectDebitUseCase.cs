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

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));
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
            LoggingHandler.LogInfo($"STARTING DIRECT DEBIT IMPORT");

            var existBatchToday = await _batchLogGateway.GetAsync(DirectDebitLabel).ConfigureAwait(false);
            if (existBatchToday != null)
            {
                LoggingHandler.LogInfo($"EXISTS A DIRECT DEBIT LOAD PROCESS TODAY");
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(0) };
            }

            const string sheetName = "Active";
            const string sheetRange = "A:C";

            var batch = await _batchLogGateway.CreateAsync(DirectDebitLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(DirectDebitLabel).ConfigureAwait(false);

            if (googleFileSettings == null)
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(0) };

            var directDebits = await _googleClientService
                .ReadSheetToEntitiesAsync<DirectDebitAuxDomain>(googleFileSettings.GoogleIdentifier, sheetName, sheetRange)
                .ConfigureAwait(false);

            if (!directDebits.Any())
            {
                LoggingHandler.LogInfo($"NO DIRECT DEBIT DATA TO IMPORT");
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
            }

            await HandleSpreadSheet(batch.Id, directDebits).ConfigureAwait(false);

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"END DIRECT DEBIT IMPORT");
            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(0) };
        }

        private async Task<GoogleFileSettingDomain> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"GETTING GOOGLE FILE SETTING FOR '{label}' LABEL");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} GOOGLE FILE SETTINGS FOUND");

            return googleFileSettings.FirstOrDefault();
        }

        private async Task HandleSpreadSheet(long batchId, IList<DirectDebitAuxDomain> directDebits)
        {
            try
            {
                await _directDebitGateway.ClearDirectDebitAuxiliary().ConfigureAwait(false);

                var skip = 0;
                var failure = false;
                List<DirectDebitAuxDomain> batchDirectDebit;

                do
                {
                    batchDirectDebit = directDebits.Skip(skip).Take(_batchSize).ToList();
                    skip += _batchSize;

                    if (!batchDirectDebit.Any()) continue;

                    var bulkResult = await _directDebitGateway.CreateBulkAsync(batchDirectDebit)
                        .ConfigureAwait(false);

                    if (bulkResult == null)
                    {
                        failure = true;
                        const string message = "FAILURE TO LOAD ALL ROWS";
                        LoggingHandler.LogError(message);
                        await _batchLogErrorGateway.CreateAsync(batchId, "ERROR", message).ConfigureAwait(false);
                        continue;
                    }
                    LoggingHandler.LogInfo($"FILE LINES CREATED {bulkResult.Count}");
                }
                while (batchDirectDebit.Any() && !failure);

                if (!failure)
                {
                    await _directDebitGateway.LoadDirectDebit(batchId).ConfigureAwait(false);
                    LoggingHandler.LogInfo("FILE SUCCESS");
                }
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleSpreadSheet)}";

                await _batchLogErrorGateway.CreateAsync(batchId, "ERROR", $"APPLICATION ERROR. NOT POSSIBLE TO LOAD DIRECT DEBITS").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} APPLICATION ERROR");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
