using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class NightlyProcessLogGateway : INightlyProcessLogGateway
    {
        private readonly IDatabaseContext _context;

        public NightlyProcessLogGateway(IDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IList<NightlyProcessLog>> GetByDateCreatedAsync(DateTime createdDate)
        {
            return await _context.NightlyProcessLogs
                .Where(log => log.DateCreated.Date == createdDate.Date)
                .OrderByDescending(log => log.DateCreated)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
