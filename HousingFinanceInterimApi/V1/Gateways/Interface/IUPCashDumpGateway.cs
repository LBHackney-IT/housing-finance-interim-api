using System.Collections.Generic;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Threading.Tasks;

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
        public Task<IList<UPCashDump>> CreateBulkAsync(long fileId, IList<string> lines);

    }

}
