using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class BatchLogGateway : IBatchLogGateway
    {
        private readonly DatabaseContext _context;

        public BatchLogGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<BatchLog> CreateAsync(string type, bool isSuccess = false)
        {
            var newBatch = new BatchLog
            {
                Type = type,
                IsSuccess = isSuccess
            };
            await _context.BatchLogs.AddAsync(newBatch).ConfigureAwait(false);

            return await _context.SaveChangesAsync().ConfigureAwait(false) == 1
                    ? newBatch
                    : null;
        }

        public async Task<bool> SetToSuccessAsync(long batchId)
        {
            var batchLog = await _context.BatchLogs.FirstOrDefaultAsync(item => item.Id == batchId)
                .ConfigureAwait(false);

            if (batchLog == null)
                return false;

            batchLog.IsSuccess = true;
            batchLog.EndTime = DateTimeOffset.Now;
            return await _context.SaveChangesAsync().ConfigureAwait(false) == 1;
        }
    }
}
