using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Exceptions;

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
            var batch = await _batchLogGateway.CreateAsync(HousingSuspenseLabel).ConfigureAwait(false);

            //Get google file settings
            const string sheetName = "Housing Benefit";
            const string sheetRange = "A:E";
            var googleFileSettings = await GetGoogleFileSetting(HousingSuspenseLabel).ConfigureAwait(false)
                                     ?? throw new GoogleFileSettingNotFoundException(HousingSuspenseLabel);

            // Get all suspense transactions & load into table
            await LoadSuspenseTransactions(batch, sheetName, sheetRange, googleFileSettings).ConfigureAwait(false);

            // Retrieve all suspense transactions from table and update spreadsheet
            var newRows = await GetSuspenseTransactionsAsRows().ConfigureAwait(false);
            await _googleClientService.UpdateSheetAsync(newRows, googleFileSettings.GoogleIdentifier, sheetName, sheetRange, true).ConfigureAwait(false);

            // Set to success
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

        private async Task LoadSuspenseTransactions(BatchLogDomain batch, string sheetName, string sheetRange, GoogleFileSettingDomain googleFileSettings)
        {
            var allSuspenseTransactions = await _googleClientService
                .ReadSheetToEntitiesAsync<SuspenseTransactionAuxDomain>(googleFileSettings.GoogleIdentifier, sheetName, sheetRange)
                .ConfigureAwait(false);

            if (allSuspenseTransactions.Any())
            {
                var filledNewAccounts = allSuspenseTransactions.Where(x => !string.IsNullOrEmpty(x.NewRentAccount)).ToList();
                await HandleSpreadSheet(batch.Id, filledNewAccounts).ConfigureAwait(false);
            }
            else
            {
                LoggingHandler.LogInfo($"No suspense cash transactions found in spreadsheet {HousingSuspenseLabel}");
            }
        }

        private async Task<List<IList<object>>> GetSuspenseTransactionsAsRows()
        {
            var suspenseTransactions = await _upCashLoadSuspenseAccountsGateway.GetHousingBenefitSuspenseTransactions().ConfigureAwait(false);

            List<IList<object>> rows = new List<IList<object>>
            {
                //HEADER
                new List<object>() { "Id", "Original Payment Ref", "Payment Date", "Amount", "New Payment Ref" }
            };

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

            return rows;
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
