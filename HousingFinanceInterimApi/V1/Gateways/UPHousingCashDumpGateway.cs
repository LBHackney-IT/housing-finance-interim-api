using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Handlers;

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
        public async Task<IList<UPHousingCashDumpDomain>> CreateBulkAsync(long fileId, IList<string> lines)
        {
            try
            {
                var listUpHousingCashDump = lines.Select(c => new UPHousingCashDump
                {
                    UPHousingCashDumpFileNameId = fileId,
                    FullText = c
                }).ToList();

                _context.UpHousingCashDumps.AddRange(listUpHousingCashDump);
                bool success = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;

                return success
                    ? listUpHousingCashDump.ToDomain()
                    : null;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

    }

}
