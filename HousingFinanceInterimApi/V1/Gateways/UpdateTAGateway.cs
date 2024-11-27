using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class UpdateTAGateway : IUpdateTAGateway
    {
        private readonly DatabaseContext _context;

        public UpdateTAGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task UpdateTADetails(string tagRef, UpdateTADomain request)
        {
            try
            {
                var uhTenancyAgreement = _context.UHTenancyAgreement.SingleOrDefault(p => p.TenancyAgreementRef == tagRef);
                var maTenancyAgreement = _context.MATenancyAgreement.SingleOrDefault(p => p.TenancyAgreementRef == tagRef);

                if (uhTenancyAgreement is not null && maTenancyAgreement is not null)
                {
                    uhTenancyAgreement.EndOfTenure = request.TenureEndDate;
                    maTenancyAgreement.EndOfTenure = request.TenureEndDate;
                }
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                LoggingHandler.LogError(ex.Message);
                LoggingHandler.LogError(ex.StackTrace);
                LoggingHandler.LogError($"Unable to upload tag_ref {tagRef} with this end date {request.TenureEndDate} as requested");
                throw;
            }
        }

    }
}
