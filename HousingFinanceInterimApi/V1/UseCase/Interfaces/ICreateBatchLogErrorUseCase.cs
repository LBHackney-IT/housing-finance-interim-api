using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The create UP cash file name use case.
    /// </summary>
    public interface ICreateBatchLogErrorUseCase
    {
        public Task<BatchLogErrorDomain> ExecuteAsync(long batchId, string type, string message);
    }

}
