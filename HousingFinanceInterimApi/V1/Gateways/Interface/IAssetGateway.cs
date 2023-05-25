using HousingFinanceInterimApi.V1.Boundary.Request;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IAssetGateway
    {
        public Task UpdateAssetDetails(UpdateAssetDetailsQuery query, UpdateAssetDetailsRequest request);
    }

}
