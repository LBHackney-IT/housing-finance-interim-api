using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The UP Cash dump gateway interface.
    /// </summary>
    public interface IUPCashDumpGateway
    {

        /// <summary>
        /// Creates bulk file line entries asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>
        /// The list of UP cash dumps.
        /// </returns>
        public Task<List<UPCashDumpDomain>> CreateBulkAsync(long fileId, IList<string> lines);

    }

}
