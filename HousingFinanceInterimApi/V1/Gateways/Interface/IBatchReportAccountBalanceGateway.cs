using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IBatchReportAccountBalanceGateway
    {
        Task<BatchReportAccountBalanceDomain> CreateAsync(BatchReportAccountBalanceDomain batchReportAccountBalanceDomain);
        Task<bool> SetToSuccessAsync(int id);
        Task<IList<BatchReportAccountBalanceDomain>> ListAsync();
    }
}
