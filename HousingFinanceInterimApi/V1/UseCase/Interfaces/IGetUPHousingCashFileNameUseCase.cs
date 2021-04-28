using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The GET UP housing cash dump file name use case.
    /// </summary>
    public interface IGetUPHousingCashFileNameUseCase
    {

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// The <see cref="UPHousingCashFileNameDomain" /> instance.
        /// </returns>
        public Task<UPHousingCashFileNameDomain> ExecuteAsync(string fileName);

    }

}
