using HousingFinanceInterimApi.V1.Infrastructure;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IBatchLogErrorGateway
    {
        public Task<BatchLogError> CreateAsync(long batchId, string type, string message);
    }
}
