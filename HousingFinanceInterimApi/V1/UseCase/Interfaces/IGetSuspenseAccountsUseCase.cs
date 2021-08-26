using System.Collections.Generic;
using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface IGetSuspenseAccountsUseCase
    {
        public Task<IList<UPCashLoadSuspenseAccountsResponse>> ListCashFileSuspenseAccountsAsync();

        public Task<IList<UPHousingCashLoadSuspenseAccountsResponse>> ListHousingFileSuspenseAccountsAsync();
    }
}
