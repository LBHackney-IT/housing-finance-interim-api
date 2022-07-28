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
    public class LoadSuspenseHousingBenefitTransactionsUseCase : ILoadSuspenseHousingBenefitTransactionsUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly ISuspenseAccountsGateway _upCashLoadSuspenseAccountsGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private const string HousingSuspenseLabel = "Housing Suspense Transactions";
        private const string Type = "Housing Benefit";

        public LoadSuspenseHousingBenefitTransactionsUseCase(
            IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            ISuspenseAccountsGateway upCashLoadSuspenseAccountsGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _upCashLoadSuspenseAccountsGateway = upCashLoadSuspenseAccountsGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting suspense housing benefit import");

            const string sheetName = "Housing Benefit";
            const string sheetRange = "A:E";

            var batch = await _batchLogGateway.CreateAsync(HousingSuspenseLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(HousingSuspenseLabel).ConfigureAwait(false);

            if (googleFileSettings == null)
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(0) };

            var allSuspenseTransactions = await _googleClientService
                .ReadSheetToEntitiesAsync<SuspenseTransactionAuxDomain>(googleFileSettings.GoogleIdentifier, sheetName, sheetRange)
                .ConfigureAwait(false);

            if (allSuspenseTransactions.Any())
            {
                var filledNewAccounts = allSuspenseTransactions.Where(x => !string.IsNullOrEmpty(x.NewRentAccount)).ToList();

                await HandleSpreadSheet(batch.Id, filledNewAccounts).ConfigureAwait(false);
            }

            var suspenseTransactions = await _upCashLoadSuspenseAccountsGateway.GetHousingBenefitSuspenseTransactions().ConfigureAwait(false);

            List<IList<object>> rows = new List<IList<object>>();

            //HEADER
            rows.Add(new List<object>() { "Id", "Original Payment Ref", "Payment Date", "Amount", "New Payment Ref" });

            //ROWS            
            foreach (var suspenseTransaction in suspenseTransactions)
            {
                rows.Add(new List<object>() {
                    suspenseTransaction.Id,
                    suspenseTransaction.RentAccount,
                    suspenseTransaction.Date.ToString("dd/MM/yyyy"),
                    suspenseTransaction.Amount,
                    "" });
            }

            await _googleClientService.UpdateSheetAsync(rows, googleFileSettings.GoogleIdentifier, sheetName, sheetRange, true).ConfigureAwait(false);

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"End suspense housing benefit import");
            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }

        private async Task<GoogleFileSettingDomain> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting Google file setting for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} Google file settings found");

            return googleFileSettings.FirstOrDefault();
        }

        private async Task HandleSpreadSheet(long batchId, IList<SuspenseTransactionAuxDomain> cashSuspenseTransactions)
        {
            try
            {
                LoggingHandler.LogInfo($"Clear aux table");
                await _upCashLoadSuspenseAccountsGateway.ClearSuspenseTransactionsAuxAuxiliary().ConfigureAwait(false);

                if (cashSuspenseTransactions.Any())
                {
                    LoggingHandler.LogInfo($"Starting bulk insert");
                    await _upCashLoadSuspenseAccountsGateway.CreateBulkAsync(cashSuspenseTransactions, Type).ConfigureAwait(false);
                }

                LoggingHandler.LogInfo($"Starting merge");
                await _upCashLoadSuspenseAccountsGateway.LoadHousingBenefitSuspenseTransactions().ConfigureAwait(false);

                LoggingHandler.LogInfo("File success");
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleSpreadSheet)}";

                await _batchLogErrorGateway.CreateAsync(batchId, "ERROR", $"Application error. Not possible to load suspense housing benefit").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
