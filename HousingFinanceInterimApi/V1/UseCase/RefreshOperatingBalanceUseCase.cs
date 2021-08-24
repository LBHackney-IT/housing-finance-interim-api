using System;
using AutoMapper;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class RefreshOperatingBalanceUseCase : IRefreshOperatingBalanceUseCase
    {
        private readonly IOperatingBalanceGateway _operatingBalanceGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        public RefreshOperatingBalanceUseCase(IOperatingBalanceGateway operatingBalanceGateway)
        {
            _operatingBalanceGateway = operatingBalanceGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"STARTING REFRESH OPERATING BALANCE");
            try
            {
                await _operatingBalanceGateway.GenerateOperatingBalance().ConfigureAwait(false);

                LoggingHandler.LogInfo($"END REFRESH OPERATING BALANCE");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                LoggingHandler.LogError($"{namespaceLabel} APPLICATION ERROR");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
