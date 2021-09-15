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
    public class AdjustmentGateway : IAdjustmentGateway
    {
        private readonly DatabaseContext _context;

        private readonly int _batchSize = 250;//Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));

        public AdjustmentGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateBulkAsync(IList<AdjustmentDomain> adjustmentDomain)
        {
            try
            {
                var adjustmentAux = adjustmentDomain.Select(a => new Adjustment
                {
                    TenancyAgreementRef = a.TenancyAgreementRef,
                    Year = a.Year,
                    Period = a.Period,
                    TransactionType = a.TransactionType,
                    TransactionSource = a.TransactionSource,
                    Amount = a.Amount,
                    TransactionDate = a.TransactionDate,
                    IsRead = false
                }).ToList();

                await _context.BulkInsertAsync(adjustmentAux, new BulkConfig { BatchSize = _batchSize }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task LoadTransactions()
        {
            try
            {
                await _context.LoadAdjustmentTransactions().ConfigureAwait(false);
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
