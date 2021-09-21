using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
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
        private readonly DatabaseContext _context;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));

        public UPHousingCashDumpGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateBulkAsync(long fileId, IList<string> lines)
        {
            try
            {
                var listUpHousingCashDump = lines.Select(c => new UPHousingCashDump
                {
                    UPHousingCashDumpFileNameId = fileId,
                    FullText = c
                }).ToList();

                await _context.BulkInsertAsync(listUpHousingCashDump, new BulkConfig { BatchSize = _batchSize }).ConfigureAwait(false);
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
