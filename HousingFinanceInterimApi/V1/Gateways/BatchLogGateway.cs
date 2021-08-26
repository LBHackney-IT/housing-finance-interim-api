using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class BatchLogGateway : IBatchLogGateway
    {
        private readonly DatabaseContext _context;

        public BatchLogGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<BatchLogDomain> CreateAsync(string type, bool isSuccess = false)
        {
            try
            {
                var newBatch = new BatchLog { Type = type, IsSuccess = isSuccess };
                await _context.BatchLogs.AddAsync(newBatch).ConfigureAwait(false);

                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1
                    ? newBatch.ToDomain()
                    : null;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<bool> SetToSuccessAsync(long batchId)
        {
            try
            {
                var batchLog = await _context.BatchLogs.FirstOrDefaultAsync(item => item.Id == batchId)
                    .ConfigureAwait(false);

                if (batchLog == null)
                    return false;

                batchLog.IsSuccess = true;
                batchLog.EndTime = DateTimeOffset.Now;
                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<IList<BatchLogDomain>> ListLastMonthAsync()
        {
            var results = await _context.BatchLogs.Where(item => item.StartTime >= DateTimeOffset.Now.AddMonths(-1))
                .ToListAsync().ConfigureAwait(false);
            return results.ToDomain();
        }
    }
}
