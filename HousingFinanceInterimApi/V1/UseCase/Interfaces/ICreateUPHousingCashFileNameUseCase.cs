using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The create UP housing cash file name use case.
    /// </summary>
    public interface ICreateUPHousingCashFileNameUseCase
    {

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The <see cref="UPHousingCashFileNameDomain"/> instance</returns>
        public Task<UPHousingCashFileNameDomain> ExecuteAsync(string fileName);

    }

}
