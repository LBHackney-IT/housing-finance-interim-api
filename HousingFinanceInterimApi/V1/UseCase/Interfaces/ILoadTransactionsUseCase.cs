using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface ILoadTransactionsUseCase
    {
        public Task<bool> LoadCashFilesAsync();
        public Task<bool> LoadHousingFilesAsync();
    }

}
