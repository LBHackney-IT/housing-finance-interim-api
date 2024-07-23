using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class AssetGateway : IAssetGateway
    {
        private readonly DatabaseContext _context;

        public AssetGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task UpdateAssetDetails(UpdateAssetDetailsQuery query, UpdateAssetDetailsRequest request)
        {
            await UpdateAssetDetailsTable(query, request).ConfigureAwait(false);
        }

        private async Task UpdateAssetDetailsTable(UpdateAssetDetailsQuery query, UpdateAssetDetailsRequest request)
        {
            try
            {
                var maPropertyToUpdate = _context.MAProperty.SingleOrDefault(p => p.PropRef == query.PropertyReference);
                var uhPropertyToUpdate = _context.UHProperty.SingleOrDefault(p => p.PropRef == query.PropertyReference);

                if (maPropertyToUpdate != null)
                {
                    maPropertyToUpdate.ShortAddress = $"{request.PostPreamble} {request.AddressLine1}";
                    maPropertyToUpdate.Address1 = $"{request.PostPreamble} {request.AddressLine1}";
                    maPropertyToUpdate.PostPreamble = request.PostPreamble ?? request.AddressLine1;

                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }

                if (uhPropertyToUpdate != null)
                {
                    uhPropertyToUpdate.ShortAddress = $"{request.PostPreamble} {request.AddressLine1}";
                    uhPropertyToUpdate.Address1 = $"{request.PostPreamble} {request.AddressLine1}";
                    uhPropertyToUpdate.PostPreamble = request.PostPreamble ?? request.AddressLine1;

                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
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
