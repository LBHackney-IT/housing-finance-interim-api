using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface ISuspenseAccountGateway
    {
        public Task<IList<UPCashLoadSuspenseAccountsDomain>> ListCashFileSuspenseAccountsAsync();

        public Task<IList<UPHousingCashLoadSuspenseAccountsDomain>> ListHousingFileSuspenseAccountsAsync();

        public Task<bool> UpdateCashLoadSuspenseAccountToResolvedAsync(long id, string newRentAccount);

        public Task<bool> UpdateHousingCashLoadSuspenseAccountToResolvedAsync(long id, string newRentAccount);
    }
}
