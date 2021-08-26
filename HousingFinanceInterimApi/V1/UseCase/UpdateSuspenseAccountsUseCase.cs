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
    public class UpdateSuspenseAccountsUseCase : IUpdateSuspenseAccountsUseCase
    {
        private readonly ISuspenseAccountGateway _suspenseAccountGateway;
        private readonly ITransactionGateway _transactionGateway;

        public UpdateSuspenseAccountsUseCase(ISuspenseAccountGateway suspenseAccountGateway, ITransactionGateway transactionGateway)
        {
            _suspenseAccountGateway = suspenseAccountGateway;
            _transactionGateway = transactionGateway;
        }

        public async Task<bool> ResolveCashFileSuspenseAccountsAsync(long id, string newRentAccount)
        {
            LoggingHandler.LogInfo($"CREATING TRANSACTION. SUSPENSE CASH FILE ID: {id}, NEW RENT ACCOUNT: {newRentAccount}");

            await _transactionGateway.CreateCashFileSuspenseAccountTransaction(id, newRentAccount)
                .ConfigureAwait(false);

            var updatedSuspenseAccount = await _suspenseAccountGateway
                .UpdateCashLoadSuspenseAccountToResolvedAsync(id, newRentAccount).ConfigureAwait(false);

            LoggingHandler.LogInfo($"{updatedSuspenseAccount}");

            return updatedSuspenseAccount;
        }

        public async Task<bool> ResolveHousingFileSuspenseAccountsAsync(long id, string newRentAccount)
        {
            LoggingHandler.LogInfo($"CREATING TRANSACTION. SUSPENSE HOUSING FILE ID: {id}, NEW RENT ACCOUNT: {newRentAccount}");

            await _transactionGateway.CreateHousingFileSuspenseAccountTransaction(id, newRentAccount)
                .ConfigureAwait(false);

            var updatedSuspenseAccount = await _suspenseAccountGateway
                .UpdateHousingCashLoadSuspenseAccountToResolvedAsync(id, newRentAccount).ConfigureAwait(false);

            LoggingHandler.LogInfo($"{updatedSuspenseAccount}");

            return updatedSuspenseAccount;
        }
    }
}
