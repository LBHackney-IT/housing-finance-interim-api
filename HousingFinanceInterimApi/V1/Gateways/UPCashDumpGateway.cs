using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The UP Cash dump gateway implementation.
    /// </summary>
    /// <seealso cref="IUPCashDumpGateway" />
    public class UPCashDumpGateway : IUPCashDumpGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCashDumpGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UPCashDumpGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates the given cash file dump line entry asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="cashDumpLine">The cash dump line.</param>
        /// <param name="isRead">if set to <c>true</c> [is read].</param>
        /// <returns>
        /// The created instance of <see cref="UPCashDump" />
        /// </returns>
        public async Task<UPCashDump> CreateAsync(int fileId, string cashDumpLine, bool isRead)
        {
            UPCashDump entry = new UPCashDump
            {
                UPCashDumpFileNameId = fileId, IsRead = isRead, FullText = cashDumpLine
            };
            await _context.UpCashDumps.AddAsync(entry).ConfigureAwait(true);

            return await _context.SaveChangesAsync().ConfigureAwait(true) == 1
                ? entry
                : null;
        }

    }

}
