using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The UP Cash dump gateway implementation.
    /// </summary>
    /// <seealso cref="IUPHousingCashDumpGateway" />
    public class UPHousingCashDumpGateway : IUPHousingCashDumpGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPHousingCashDumpGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UPHousingCashDumpGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates bulk file line entries asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>
        /// The list of UP cash dumps.
        /// </returns>
        public async Task<IList<UPHousingCashDump>> CreateBulkAsync(long fileId, IList<string> lines)
        {
            IList<UPHousingCashDump> results = new List<UPHousingCashDump>();

            foreach (string line in lines)
            {
                UPHousingCashDump entry = new UPHousingCashDump
                {
                    UPHousingCashDumpFileNameId = fileId, FullText = line
                };
                await _context.UpHousingCashDumps.AddAsync(entry).ConfigureAwait(false);
                results.Add(entry);
            }

            bool success = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;

            return success
                ? results
                : null;
        }

    }

}
