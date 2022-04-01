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

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting charges transactions import");
            var batch = await _batchLogGateway.CreateAsync(_label).ConfigureAwait(false);
            try
            {
                LoggingHandler.LogInfo($"Convert ChargesHistory in Transactions");
                await _transactionGateway.LoadChargesTransactions().ConfigureAwait(false);

                await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
                LoggingHandler.LogInfo($"End charges transactions import");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                await _batchLogErrorGateway
                    .CreateAsync(batch.Id, "ERROR", $"Application error. Not possible to load charges transactions")
                    .ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }

        public async Task<StepResponse> ExecuteOnDemandAsync(DateTime startDate, DateTime endDate)
        {
            LoggingHandler.LogInfo($"Starting charges transactions on demand import");
            var batch = await _batchLogGateway.CreateAsync(_label).ConfigureAwait(false);
            try
            {
                while (startDate <= endDate)
                {
                    LoggingHandler.LogInfo($"Load ChargesHistory table");
                    await _chargesGateway.LoadChargesHistory(startDate.Year).ConfigureAwait(false);

                    LoggingHandler.LogInfo($"Convert ChargesHistory in transactions");
                    await _transactionGateway.LoadChargesTransactions().ConfigureAwait(false);

                    startDate = startDate.Date.AddDays(1);
                }

                await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
                LoggingHandler.LogInfo($"End cash file transactions on demand import");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                await _batchLogErrorGateway
                    .CreateAsync(batch.Id, "ERROR", $"Application error. Not possible to load charges transactions on demand")
                    .ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
