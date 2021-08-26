using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IBatchLogGateway
    {
        public Task<BatchLogDomain> CreateAsync(string type, bool isSuccess = false);

        public Task<bool> SetToSuccessAsync(long batchId);

        public Task<IList<BatchLogDomain>> ListLastMonthAsync();
    }
}
