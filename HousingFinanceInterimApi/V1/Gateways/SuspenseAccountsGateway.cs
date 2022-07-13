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
    public class SuspenseAccountsGateway : ISuspenseAccountsGateway
    {
        private readonly DatabaseContext _context;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));

        public SuspenseAccountsGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateBulkAsync(IList<SuspenseTransactionAuxDomain> suspenseTransactionsAuxDomain, string type)
        {
            try
            {
                var suspenseTransactionsAux = suspenseTransactionsAuxDomain.Select(c => new SuspenseTransactionAux
                {
                    IdSuspenseTransaction = c.Id,
                    RentAccount = c.RentAccount,
                    Type = type,
                    Date = c.Date,
                    Amount = c.Amount,
                    NewRentAccount = c.NewRentAccount
                }).ToList();

                await _context.BulkInsertAsync(suspenseTransactionsAux, new BulkConfig { BatchSize = _batchSize }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task ClearSuspenseTransactionsAuxAuxiliary()
        {
            try
            {
                await _context.TruncateSuspenseTransactionAuxiliary().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<IList<SuspenseTransactionAuxDomain>> GetCashSuspenseTransactions()
        {
            try
            {
                var suspense = await _context.GetCashSuspenseTransactions().ConfigureAwait(false);

                return suspense?.Select(d => new SuspenseTransactionAuxDomain()
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
                await _context.LoadCashSuspenseTransactions().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<IList<SuspenseTransactionAuxDomain>> GetHousingBenefitSuspenseTransactions()
        {
            try
            {
                var suspense = await _context.GetHousingBenefitSuspenseTransactions().ConfigureAwait(false);

                return suspense?.Select(d => new SuspenseTransactionAuxDomain()
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

        public async Task LoadHousingBenefitSuspenseTransactions()
        {
            try
            {
                await _context.LoadHousingBenefitSuspenseTransactions().ConfigureAwait(false);
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
