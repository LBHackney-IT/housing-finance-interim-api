using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface ICheckExistFileUseCase
    {
        public Task<StepResponse> ExecuteAsync(string label);
    }

}
