using HousingFinanceInterimApi.V1.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The create bulk housing cash dumps use case.
    /// </summary>
    interface ICreateBulkHousingCashDumpsUseCase
    {

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>The created UP housing cash dump domain objects.</returns>
        public Task<IList<UPHousingCashDumpDomain>> ExecuteAsync(long fileId, IList<string> lines);

    }

}
