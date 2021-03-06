using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The GET UP cash dump file name use case.
    /// </summary>
    public interface IGetUPCashFileNameUseCase
    {

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// The <see cref="UPCashFileNameDomain" /> instance.
        /// </returns>
        public Task<UPCashFileNameDomain> ExecuteAsync(string fileName);

    }

}
