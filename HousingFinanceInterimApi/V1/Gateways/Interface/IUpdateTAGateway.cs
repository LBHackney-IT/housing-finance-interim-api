using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IUpdateTAGateway
    {
        Task UpdateTADetails(string tagRef, UpdateTADomain request);
    }
}
