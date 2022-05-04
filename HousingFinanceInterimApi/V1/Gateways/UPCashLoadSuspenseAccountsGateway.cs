using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Handlers;
using System.Collections.Generic;
using HousingFinanceInterimApi.V1.Domain;
using System.Linq;
using EFCore.BulkExtensions;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class UPCashLoadSuspenseAccountsGateway : IUPCashLoadSuspenseAccountsGateway
    {
        private readonly DatabaseContext _context;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));

        public UPCashLoadSuspenseAccountsGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateBulkAsync(IList<CashSuspenseTransactionAuxDomain> cashSuspenseDomain)
        {
            try
            {
                var cashSuspenseTransactionAux = cashSuspenseDomain.Select(c => new CashSuspenseTransactionAux
                {
                    IdSuspenseTransaction = c.Id,
                    RentAccount = c.RentAccount,
                    Date = c.Date,
                    Amount = c.Amount,
                    NewRentAccount = c.NewRentAccount
                }).ToList();

                await _context.BulkInsertAsync(cashSuspenseTransactionAux, new BulkConfig { BatchSize = _batchSize }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task ClearCashSuspenseTransactionsAuxAuxiliary()
        {
            try
            {
                await _context.TruncateCashSuspenseTransactionAuxiliary().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<IList<CashSuspenseTransactionAuxDomain>> GetCashSuspenseTransactions()
        {
            try
            {
                var suspense = await _context.GetCashSuspenseTransactions().ConfigureAwait(false);

                return suspense?.Select(d => new CashSuspenseTransactionAuxDomain()
                {
                    Id = d.Id,
                    RentAccount = d.RentAccount,
                    Date = d.PaymentDate,
                    Amount = d.Amount,
                    NewRentAccount = d.NewRentAccount
                }).ToList();
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task LoadCashSuspenseTransactions()
        {
            try
            {
                await _context.LoadcashSuspenseTransactions().ConfigureAwait(false);
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
