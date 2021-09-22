using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public interface ILoadAdjustmentUseCase
    {
        public Task<StepResponse> ExecuteAsync();
    }
}
