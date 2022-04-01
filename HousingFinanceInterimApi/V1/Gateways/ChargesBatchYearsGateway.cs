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
    public class ChargesBatchYearsGateway : IChargesBatchYearsGateway
    {
        private readonly DatabaseContext _context;

        public ChargesBatchYearsGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ChargesBatchYearDomain> CreateAsync(int year, bool isRead = false)
        {
            try
            {
                var newBatch = new ChargesBatchYear { ProcessingDate = DateTime.Now, Year = year, IsRead = isRead };
                await _context.ChargesBatchYears.AddAsync(newBatch).ConfigureAwait(false);

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

        public async Task<bool> SetToSuccessAsync(int year)
        {
            try
            {
                var chargesBatchYears = await _context.ChargesBatchYears.FirstOrDefaultAsync(item => item.ProcessingDate == DateTime.Now.Date && item.Year == year)
                    .ConfigureAwait(false);

                if (chargesBatchYears == null)
                    return false;

                chargesBatchYears.IsRead = true;
                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<bool> ExistDataForToday()
        {
            var results = await _context.ChargesBatchYears.Where(item => item.ProcessingDate == DateTime.Now.Date)
                .ToListAsync().ConfigureAwait(false);
            return results.Any();
        }

        public async Task<ChargesBatchYearDomain> GetPendingYear()
        {
            var results = await _context.ChargesBatchYears.Where(item => item.ProcessingDate == DateTime.Now.Date && item.IsRead == false).OrderBy(o => o.Year)
                .ToListAsync().ConfigureAwait(false);

            return results.Any() ? results.First().ToDomain() : null;
        }
    }
}
