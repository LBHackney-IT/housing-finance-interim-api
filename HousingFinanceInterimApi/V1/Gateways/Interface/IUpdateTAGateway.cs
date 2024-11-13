using HousingFinanceInterimApi.V1.Boundary.Request;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IUpdateTAGateway
    {
        Task UpdateTADetails(UpdateTAQuery query, UpdateTARequest request);
    }
}
