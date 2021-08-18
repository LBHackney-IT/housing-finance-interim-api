using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class UPCashDumpGateway : IUPCashDumpGateway
    {
        private readonly DatabaseContext _context;

        public UPCashDumpGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<UPCashDumpDomain>> CreateBulkAsync(long fileId, IList<string> lines)
        {
            try
            {
                var listUpCashDump = lines.Select(c => new UPCashDump
                {
                    UPCashDumpFileNameId = fileId,
                    FullText = c
                }).ToList();

                _context.UpCashDumps.AddRange(listUpCashDump);
                bool success = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;

                return success
                    ? listUpCashDump.ToDomain()
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
