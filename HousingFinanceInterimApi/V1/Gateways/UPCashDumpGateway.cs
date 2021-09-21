using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class UPCashDumpGateway : IUPCashDumpGateway
    {
        private readonly DatabaseContext _context;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));

        public UPCashDumpGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateBulkAsync(long fileId, IList<string> lines)
        {
            try
            {
                var listUpCashDump = lines.Select(c => new UPCashDump
                {
                    UPCashDumpFileNameId = fileId,
                    FullText = c
                }).ToList();

                await _context.BulkInsertAsync(listUpCashDump, new BulkConfig { BatchSize = _batchSize }).ConfigureAwait(false);
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
