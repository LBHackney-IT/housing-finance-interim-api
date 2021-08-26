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
    public class SuspenseAccountGateway : ISuspenseAccountGateway
    {
        private readonly DatabaseContext _context;

        public SuspenseAccountGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IList<UPCashLoadSuspenseAccountsDomain>> ListCashFileSuspenseAccountsAsync()
        {
            var results = await _context.UPCashLoadSuspenseAccounts.Where(
                    item => item.IsResolved == false)
                .ToListAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<IList<UPHousingCashLoadSuspenseAccountsDomain>> ListHousingFileSuspenseAccountsAsync()
        {
            var results = await _context.UPHousingCashLoadSuspenseAccounts.Where(
                    item => item.IsResolved == false)
                .ToListAsync().ConfigureAwait(false);
            return results.ToDomain();
        }
    }
}
