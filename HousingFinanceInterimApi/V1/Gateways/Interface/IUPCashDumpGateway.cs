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
        /// Creates the given cash file dump line entry asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="cashDumpLine">The cash dump line.</param>
        /// <param name="isRead">if set to <c>true</c> [is read].</param>
        /// <returns>The created instance of <see cref="UPCashDump"/></returns>
        public Task<UPCashDump> CreateAsync(int fileId, string cashDumpLine, bool isRead);

    }

}
