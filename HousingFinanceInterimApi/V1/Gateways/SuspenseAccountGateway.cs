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

        public async Task<UPCashLoadSuspenseAccountsDomain> GetCashFileSuspenseAccountAsync(long id)
        {
            var results =
                await _context.UPCashLoadSuspenseAccounts.FirstOrDefaultAsync(item => item.Id.Equals(id)).ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<UPHousingCashLoadSuspenseAccountsDomain> GetHousingFileSuspenseAccountAsync(long id)
        {
            var results =
                await _context.UPHousingCashLoadSuspenseAccounts.FirstOrDefaultAsync(item => item.Id.Equals(id)).ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<bool> UpdateCashLoadSuspenseAccountToResolvedAsync(long id, string newRentAccount)
        {
            try
            {
                var cashLoadSuspenseAccount = await _context.UPCashLoadSuspenseAccounts.FirstOrDefaultAsync(item => item.Id == id).ConfigureAwait(false);

                if (cashLoadSuspenseAccount == null)
                    return false;

                cashLoadSuspenseAccount.NewRentAccount = newRentAccount;
                cashLoadSuspenseAccount.IsResolved = true;
                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<bool> UpdateHousingCashLoadSuspenseAccountToResolvedAsync(long id, string newRentAccount)
        {
            try
            {
                var housingCashLoadSuspenseAccount = await _context.UPHousingCashLoadSuspenseAccounts.FirstOrDefaultAsync(item => item.Id == id).ConfigureAwait(false);

                if (housingCashLoadSuspenseAccount == null)
                    return false;

                housingCashLoadSuspenseAccount.NewRentAccount = newRentAccount;
                housingCashLoadSuspenseAccount.IsResolved = true;
                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
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
