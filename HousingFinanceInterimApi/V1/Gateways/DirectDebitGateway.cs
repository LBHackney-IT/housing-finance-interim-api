using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class DirectDebitGateway : IDirectDebitGateway
    {
        private readonly DatabaseContext _context;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));

        public DirectDebitGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateBulkAsync(IList<DirectDebitAuxDomain> directDebitsDomain)
        {
            try
            {
                var directDebitsAux = directDebitsDomain.Select(dd => new DirectDebitAux
                {
                    RentAccount = dd.RentAccount,
                    Date = dd.Date,
                    Amount = dd.Amount
                }).ToList();

                await _context.BulkInsertAsync(directDebitsAux, new BulkConfig { BatchSize = _batchSize }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task ClearDirectDebitAuxiliary()
        {
            try
            {
                await _context.TruncateDirectDebitAuxiliary().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task LoadDirectDebit(long batchLogId)
        {
            try
            {
                await _context.LoadDirectDebit(batchLogId).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task LoadDirectDebitHistory(DateTime? processingDate)
        {
            try
            {
                await _context.LoadDirectDebitHistory(processingDate).ConfigureAwait(false);
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
