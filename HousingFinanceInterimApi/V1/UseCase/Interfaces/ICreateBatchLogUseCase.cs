using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The create UP cash file name use case.
    /// </summary>
    public interface ICreateBatchLogUseCase
    {
        public Task<BatchLogDomain> ExecuteAsync(string type);
    }

}
