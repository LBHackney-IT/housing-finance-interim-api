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
    public class LoadChargesTransactionsUseCase : ILoadChargesTransactionsUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IChargesGateway _chargesGateway;
        private readonly ITransactionGateway _transactionGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _label = "ChargesTransaction";

        public LoadChargesTransactionsUseCase(
            IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IChargesGateway chargesGateway,
            ITransactionGateway transactionGateway)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _chargesGateway = chargesGateway;
            _transactionGateway = transactionGateway;
        }

        public async Task<StepResponse> ExecuteAsync(DateTime? processingDate = null)
        {
            LoggingHandler.LogInfo($"STARTING CHARGES TRANSACTIONS IMPORT");
            var batch = await _batchLogGateway.CreateAsync(_label).ConfigureAwait(false);
            try
            {
                LoggingHandler.LogInfo($"LOAD ChargesHistory TABLE");

                await _chargesGateway.LoadChargesHistory(processingDate).ConfigureAwait(false);
                LoggingHandler.LogInfo($"CONVERT ChargesHistory IN TRANSACTIONS");
                await _transactionGateway.LoadChargesTransactions().ConfigureAwait(false);

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

                await _batchLogErrorGateway.CreateAsync(batch.Id, "ERROR", $"APPLICATION ERROR. NOT POSSIBLE TO LOAD CHARGES TRANSACTIONS").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} APPLICATION ERROR");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
