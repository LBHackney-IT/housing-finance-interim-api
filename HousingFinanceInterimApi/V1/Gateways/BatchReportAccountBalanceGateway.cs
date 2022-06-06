using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Handlers;
using Microsoft.EntityFrameworkCore;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class BatchReportAccountBalanceGateway : IBatchReportAccountBalanceGateway
    {
        private readonly DatabaseContext _context;

        public BatchReportAccountBalanceGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<BatchReportAccountBalanceDomain> CreateAsync(BatchReportAccountBalanceDomain batchReportAccountBalanceDomain)
        {
            try
            {
                var batchReportAccountBalance = batchReportAccountBalanceDomain.ToDatabase();
                await _context.BatchReportAccountBalances.AddAsync(batchReportAccountBalance).ConfigureAwait(false);

                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1
                    ? batchReportAccountBalance.ToDomain()
                    : null;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<bool> SetToSuccessAsync(int id, string link)
        {
            try
            {
                var batch = await _context.BatchReportAccountBalances.FirstOrDefaultAsync(item => item.Id == id)
                    .ConfigureAwait(false);

                if (batch == null)
                    return false;

                batch.Link = link;
                batch.IsSuccess = true;
                batch.EndTime = DateTimeOffset.Now;
                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<IList<BatchReportAccountBalanceDomain>> ListAsync()
        {
            var results = await _context.BatchReportAccountBalances.ToListAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<IList<BatchReportAccountBalanceDomain>> ListPendingAsync()
        {
            var results = await _context.BatchReportAccountBalances
                .Where(x => !x.IsSuccess)
                .ToListAsync()
                .ConfigureAwait(false);

            return results.ToDomain();
        }
    }
}
