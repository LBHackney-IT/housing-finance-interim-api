using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IBatchLogErrorGateway
    {
        public Task<BatchLogErrorDomain> CreateAsync(long batchId, string type, string message);
    }
}
