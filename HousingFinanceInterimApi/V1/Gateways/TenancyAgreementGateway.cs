using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class TenancyAgreementGateway : ITenancyAgreementGateway
    {

        private readonly DatabaseContext _context;

        public TenancyAgreementGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<TenancyAgreementAuxDomain>> CreateBulkAsync(
            IList<TenancyAgreementAuxDomain> tenancyAgreementAuxDomain)
        {
            try
            {
                var tenancyAgreementAux = tenancyAgreementAuxDomain.Select(t => new TenancyAgreementAux()
                {
                    TenancyAgreementRef = t.TenancyAgreementRef,
                    RentAccount = t.RentAccount,
                    RentGroup = t.RentGroup,
                    Tenure = t.Tenure,
                    StartDate = t.StartDate ?? null,
                    EndDate = t.EndDate ?? null,
                    PropertyRef = t.PropertyRef,
                    ShortAddress = t.ShortAddress,
                    Address = t.Address,
                    PostCode = t.PostCode,
                    NumBedrooms = t.NumBedrooms ?? 0,
                    HouseholdRef = t.HouseholdRef,
                    Title = t.Title,
                    Forename = t.Forename,
                    Surname = t.Surname,
                    Age = t.Age ?? 0,
                    ContactName = t.ContactName,
                    ContactAddress = t.ContactAddress,
                    ContactPostCode = t.ContactPostCode,
                    ContactPhone = t.ContactPhone
                }).ToList();

                _context.TenancyAgreementsAux.AddRange(tenancyAgreementAux);
                bool success = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;

                return success
                    ? tenancyAgreementAux.ToDomain()
                    : null;
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

        public async Task RefreshTenancyAgreement()
        {
            try
            {
                await _context.RefreshTenancyAgreementTables().ConfigureAwait(false);
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
