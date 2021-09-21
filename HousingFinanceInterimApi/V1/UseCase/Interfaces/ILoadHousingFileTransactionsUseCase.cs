using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface ILoadHousingFileTransactionsUseCase
    {
        public Task<StepResponse> ExecuteAsync();
    }

}
