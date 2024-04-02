using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class LoadCashFileTransactionsUseCase : ILoadCashFileTransactionsUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IUPCashLoadGateway _upCashLoadGateway;
        private readonly ITransactionGateway _transactionGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _label = "CashFileTransaction";

        public LoadCashFileTransactionsUseCase(
            IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IUPCashLoadGateway upCashLoadGateway,
            ITransactionGateway transactionGateway)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _upCashLoadGateway = upCashLoadGateway;
            _transactionGateway = transactionGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting cash file transactions import");
            var batch = await _batchLogGateway.CreateAsync(_label).ConfigureAwait(false);
            try
            {
                LoggingHandler.LogInfo($"Load UpCashLoad table");
                await _upCashLoadGateway.LoadCashFiles().ConfigureAwait(false);
                LoggingHandler.LogInfo($"Convert UpCashLoad in transactions");
                await _transactionGateway.LoadCashFilesTransactions().ConfigureAwait(false);

                await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
                LoggingHandler.LogInfo($"End cash file transactions import");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                await _batchLogErrorGateway.CreateAsync(batch?.Id, "ERROR", $"Application error. Not possible to load cash files transactions").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
