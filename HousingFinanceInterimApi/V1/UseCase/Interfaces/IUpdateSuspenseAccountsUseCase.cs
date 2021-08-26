using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface IUpdateSuspenseAccountsUseCase
    {
        public Task<bool> ResolveCashFileSuspenseAccountsAsync(long id, string newRentAccount);

        public Task<bool> ResolveHousingFileSuspenseAccountsAsync(long id, string newRentAccount);
    }

}
