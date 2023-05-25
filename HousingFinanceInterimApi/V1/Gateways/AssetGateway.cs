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
            await _context.UpdateAssetDetails(query, request).ConfigureAwait(false);
        }
    }
}
