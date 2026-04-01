using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class RefreshCurrentBalanceUseCase : IRefreshCurrentBalanceUseCase
    {
        private readonly ICurrentBalanceGateway _currentBalanceGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        public RefreshCurrentBalanceUseCase(ICurrentBalanceGateway currentBalanceGateway)
        {
            _currentBalanceGateway = currentBalanceGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting refresh current balance");
            try
            {
                await _currentBalanceGateway.UpdateCurrentBalance().ConfigureAwait(false);

                LoggingHandler.LogInfo($"End refresh current balance");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
