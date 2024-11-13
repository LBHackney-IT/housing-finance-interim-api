using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Infrastructure;
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

        public async Task UpdateTADetails(UpdateTAQuery query, UpdateTARequest request)
        {
            try
            {
                var uhTenancyAgreement = _context.UHTenancyAgreement.SingleOrDefault(p => p.PropRef == query.PropertyReference);
                var maTenancyAgreement = _context.MATenancyAgreement.SingleOrDefault(p => p.PropRef == query.PropertyReference);

                uhTenancyAgreement.Eot = request.TenureEndDate;
                maTenancyAgreement.Eot = request.TenureEndDate;

                if (request.TenureEndDate is not null)
                {
                    uhTenancyAgreement.Terminated = true;
                    uhTenancyAgreement.Present = false;
                }
                else
                {
                    uhTenancyAgreement.Terminated = false;
                    uhTenancyAgreement.Present = true;
                }
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                LoggingHandler.LogError(ex.Message);
                LoggingHandler.LogError(ex.StackTrace);
                throw;
            }
        }

    }
}
