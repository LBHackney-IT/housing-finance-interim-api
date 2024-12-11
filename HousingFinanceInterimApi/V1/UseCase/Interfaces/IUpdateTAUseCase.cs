using HousingFinanceInterimApi.V1.Boundary.Request;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface IUpdateTAUseCase
    {
        Task ExecuteAsync(string tagRef, UpdateTARequest request);
    }
}
