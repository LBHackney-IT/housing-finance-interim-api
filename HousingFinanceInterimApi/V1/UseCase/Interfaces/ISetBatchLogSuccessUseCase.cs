using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface ISetBatchLogSuccessUseCase
    {
        public Task ExecuteAsync(long batchId);
    }
}
