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
    public class BatchLogErrorGateway : IBatchLogErrorGateway
    {
        private readonly DatabaseContext _context;

        public BatchLogErrorGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<BatchLogErrorDomain> CreateAsync(long batchId, string type, string message)
        {
            try
            {
                var newBatchError = new BatchLogError()
                {
                    Type = type,
                    BatchLogId = batchId,
                    Message = message
                };
                await _context.BatchLogErrors.AddAsync(newBatchError).ConfigureAwait(false);

                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1
                    ? newBatchError.ToDomain()
                    : null;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<IList<BatchLogErrorDomain>> ListLastMonthAsync()
        {
            var results = await _context.BatchLogErrors.Where(item => item.Timestamp >= DateTimeOffset.Now.AddMonths(-1))
                .ToListAsync().ConfigureAwait(false);
            return results.ToDomain();
        }
    }
}
