using HousingFinanceInterimApi.V1.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The create bulk cash dumps use case.
    /// </summary>
    interface ICreateBulkCashDumpsUseCase
    {

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>The created UP cash dump domain objects.</returns>
        public Task<IList<UPCashDumpDomain>> ExecuteAsync(long fileId, IList<string> lines);

    }

}
