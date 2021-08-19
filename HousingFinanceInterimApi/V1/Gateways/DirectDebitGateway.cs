using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class DirectDebitGateway : IDirectDebitGateway
    {
        private readonly DatabaseContext _context;

        public DirectDebitGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<DirectDebitAuxDomain>> CreateBulkAsync(IList<DirectDebitAuxDomain> directDebitsDomain)
        {
            try
            {
                var directDebits = directDebitsDomain.Select(dd => new DirectDebitAux
                {
                    RentAccount = dd.RentAccount,
                    DueDay = dd.DueDay,
                    Amount = dd.Amount
                }).ToList();

                _context.DirectDebitsAux.AddRange(directDebits);
                bool success = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;

                return success
                    ? directDebits.ToDomain()
                    : null;
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
