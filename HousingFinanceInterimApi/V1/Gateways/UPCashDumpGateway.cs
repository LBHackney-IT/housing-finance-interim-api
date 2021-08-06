using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;
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
        /// Creates bulk file line entries asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>
        /// The list of UP cash dumps.
        /// </returns>
        public async Task<IList<UPCashDump>> CreateBulkAsync(long fileId, IList<string> lines)
        {
            var destObject = lines.Select(c => new UPCashDump
            {
                UPCashDumpFileNameId = fileId,
                FullText = c
            }).ToList();

            _context.UpCashDumps.AddRange(destObject);
            bool success = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;

            //IList<UPCashDump> results = new List<UPCashDump>();

            //foreach (string line in lines)
            //{
            //    UPCashDump entry = new UPCashDump
            //    {
            //        UPCashDumpFileNameId = fileId,
            //        FullText = line
            //    };
            //    //await _context.UpCashDumps.AddAsync(entry).ConfigureAwait(false);
            //    results.Add(entry);
            //}

            //_context.UpCashDumps.AddRange(results);
            //bool success = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;

            return success
                ? destObject
                : null;
        }

    }

}
