using HousingFinanceInterimApi.V1.Infrastructure;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IBatchLogGateway
    {
        public Task<BatchLog> CreateAsync(string type, bool isSuccess = false);

        public Task<bool> SetToSuccessAsync(long batchId);

    }
}
