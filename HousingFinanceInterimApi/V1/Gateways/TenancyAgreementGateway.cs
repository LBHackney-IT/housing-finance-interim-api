using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class TenancyAgreementGateway : ITenancyAgreementGateway
    {
        private readonly DatabaseContext _context;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));

        public TenancyAgreementGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateBulkAsync(
            IList<TenancyAgreementAuxDomain> tenancyAgreementAuxDomain, string rentGroup)
        {
            try
            {
                var tenancyAgreementAux = tenancyAgreementAuxDomain.Select(t => new TenancyAgreementAux()
                {
                    PaymentRef = t.PaymentRef,
                    RentGroup = rentGroup,
                    Tenure = t.Tenure,
                    StartDate = t.StartDate ?? null,
                    EndDate = t.EndDate ?? null,
                    PropertyRef = t.PropertyRef,
                    ShortAddress = t.ShortAddress,
                    Address = t.Address,
                    PostCode = t.PostCode,
                    NumBedrooms = t.NumBedrooms ?? 0,
                    Title = t.Title,
                    Forename = t.Forename,
                    Surname = t.Surname,
                    DateOfBirth = t.DateOfBirth
                }).ToList();

                await _context.BulkInsertAsync(tenancyAgreementAux, new BulkConfig { BatchSize = _batchSize }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task ClearTenancyAgreementAuxiliary()
        {
            try
            {
                await _context.TruncateTenancyAgreementAuxiliary().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task RefreshTenancyAgreement(long batchLogId)
        {
            try
            {
                await _context.RefreshTenancyAgreementTables(batchLogId).ConfigureAwait(false);
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
