using HousingFinanceInterimApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface ILogParserUseCase
    {
        Task<StepResponse> ExecuteAsync();
    }
}
