using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class LoadHousingFileTransactionsUseCase : ILoadHousingFileTransactionsUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IUPHousingCashLoadGateway _upHousingCashLoadGateway;
        private readonly ITransactionGateway _transactionGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _label = "HousingBenefitFileTransaction";

        public LoadHousingFileTransactionsUseCase(
            IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IUPHousingCashLoadGateway upHousingCashLoadGateway,
            ITransactionGateway transactionGateway)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _upHousingCashLoadGateway = upHousingCashLoadGateway;
            _transactionGateway = transactionGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting housing benefit file transactions import");
            var batch = await _batchLogGateway.CreateAsync(_label).ConfigureAwait(false);
            try
            {
                LoggingHandler.LogInfo($"Load UpHousingCashLoad table");
                await _upHousingCashLoadGateway.LoadHousingFiles().ConfigureAwait(false);
                LoggingHandler.LogInfo($"Convert UpHousingCashLoad in transactions");
                await _transactionGateway.LoadHousingFilesTransactions().ConfigureAwait(false);

                await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
                LoggingHandler.LogInfo($"End housing benefit file transactions import");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                await _batchLogErrorGateway.CreateAsync(batch.Id, "ERROR", $"Application error. Not possible to load housing files transactions").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
