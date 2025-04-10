using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface IGenerateReportUseCase
    {
        Task<StepResponse> ExecuteAsync();
    }

}
