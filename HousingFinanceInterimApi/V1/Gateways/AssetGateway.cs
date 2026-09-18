using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
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
            var fullAddress = $"{request.AddressLine1}, {request.AddressLine2}, {request.AddressLine3}, {request.AddressLine4}";
            await _context.UpdateAssetDetails(query.PropertyReference, request.PostPreamble, fullAddress).ConfigureAwait(false);
        }
    }
}
