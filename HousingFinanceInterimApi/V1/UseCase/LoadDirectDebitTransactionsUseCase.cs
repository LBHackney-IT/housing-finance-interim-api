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
    public class LoadDirectDebitTransactionsUseCase : ILoadDirectDebitTransactionsUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IDirectDebitGateway _directDebitGateway;
        private readonly ITransactionGateway _transactionGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _label = "DirectDebitTransaction";

        public LoadDirectDebitTransactionsUseCase(
            IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IDirectDebitGateway directDebitGateway,
            ITransactionGateway transactionGateway)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _directDebitGateway = directDebitGateway;
            _transactionGateway = transactionGateway;
        }

        public async Task<StepResponse> ExecuteAsync(DateTime? processingDate = null)
        {
            LoggingHandler.LogInfo($"STARTING DIRECT DEBIT TRANSACTIONS IMPORT");
            var batch = await _batchLogGateway.CreateAsync(_label).ConfigureAwait(false);
            try
            {
                LoggingHandler.LogInfo($"LOAD DirectDebitHistory TABLE");

                await _directDebitGateway.LoadDirectDebitHistory(processingDate).ConfigureAwait(false);
                LoggingHandler.LogInfo($"CONVERT DirectDebitHistory IN TRANSACTIONS");
                await _transactionGateway.LoadDirectDebitTransactions().ConfigureAwait(false);

                await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
                LoggingHandler.LogInfo($"END CASH FILE TRANSACTIONS IMPORT");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                await _batchLogErrorGateway.CreateAsync(batch.Id, "ERROR", $"APPLICATION ERROR. NOT POSSIBLE TO LOAD DIRECT DEBIT TRANSACTIONS").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} APPLICATION ERROR");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }

        public async Task<StepResponse> ExecuteOnDemandAsync(DateTime startDate, DateTime endDate)
        {
            LoggingHandler.LogInfo($"STARTING DIRECT DEBIT TRANSACTIONS ON DEMAND IMPORT");
            var batch = await _batchLogGateway.CreateAsync(_label).ConfigureAwait(false);
            try
            {
                while (startDate <= endDate)
                {
                    LoggingHandler.LogInfo($"LOAD DirectDebitHistory TABLE");
                    await _directDebitGateway.LoadDirectDebitHistory(startDate).ConfigureAwait(false);

                    LoggingHandler.LogInfo($"CONVERT DirectDebitHistory IN TRANSACTIONS");
                    await _transactionGateway.LoadDirectDebitTransactions().ConfigureAwait(false);

                    startDate = startDate.Date.AddDays(1);
                }

                await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
                LoggingHandler.LogInfo($"END DIRECT DEBIT TRANSACTIONS ON DEMAND IMPORT");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                await _batchLogErrorGateway.CreateAsync(batch.Id, "ERROR", $"APPLICATION ERROR. NOT POSSIBLE TO LOAD DIRECT DEBIT TRANSACTIONS ON DEMAND").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} APPLICATION ERROR");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
