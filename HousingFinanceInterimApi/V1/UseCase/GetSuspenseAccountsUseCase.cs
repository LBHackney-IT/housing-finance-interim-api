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
    public class GetSuspenseAccountsUseCase : IGetSuspenseAccountsUseCase
    {
        private readonly ISuspenseAccountGateway _suspenseAccountGateway;

        public GetSuspenseAccountsUseCase(ISuspenseAccountGateway suspenseAccountGateway)
        {
            _suspenseAccountGateway = suspenseAccountGateway;
        }

        public async Task<IList<UPCashLoadSuspenseAccountsResponse>> ListCashFileSuspenseAccountsAsync()
        {
            LoggingHandler.LogInfo($"GETTING CASH FILES SUSPENSE ACCOUNTS");

            var suspenseAccounts =
                await _suspenseAccountGateway.ListCashFileSuspenseAccountsAsync().ConfigureAwait(false);

            LoggingHandler.LogInfo($"{suspenseAccounts}");

            return suspenseAccounts.ToResponse();
        }

        public async Task<IList<UPHousingCashLoadSuspenseAccountsResponse>> ListHousingFileSuspenseAccountsAsync()
        {
            LoggingHandler.LogInfo($"GETTING HOUSING FILES SUSPENSE ACCOUNTS");

            var suspenseAccounts =
                await _suspenseAccountGateway.ListHousingFileSuspenseAccountsAsync().ConfigureAwait(false);

            LoggingHandler.LogInfo($"{suspenseAccounts}");

            return suspenseAccounts.ToResponse();
        }
    }
}
