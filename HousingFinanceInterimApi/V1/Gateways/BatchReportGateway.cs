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
    public class BatchReportGateway : IBatchReportGateway
    {
        private readonly DatabaseContext _context;

        public BatchReportGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<BatchReportDomain> CreateAsync(BatchReportDomain batchReportAccountBalanceDomain)
        {
            try
            {
                var batchReportAccountBalance = batchReportAccountBalanceDomain.ToDatabase();
                await _context.BatchReports.AddAsync(batchReportAccountBalance).ConfigureAwait(false);

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
                var batch = await _context.BatchReports.FirstOrDefaultAsync(item => item.Id == id)
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

        public async Task<IList<BatchReportDomain>> ListAsync(string reportName)
        {
            var results = await _context.BatchReports
                .Where(r => r.ReportName == reportName)
                .ToListAsync()
                .ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<IList<BatchReportDomain>> ListPendingAsync()
        {
            var results = await _context.BatchReports
                .Where(r => !r.IsSuccess)
                .ToListAsync()
                .ConfigureAwait(false);

            return results.ToDomain();
        }
    }
}
