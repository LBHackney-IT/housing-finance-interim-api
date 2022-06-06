using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IBatchReportAccountBalanceGateway
    {
        Task<BatchReportAccountBalanceDomain> CreateAsync(BatchReportAccountBalanceDomain batchReportAccountBalanceDomain);
        Task<bool> SetToSuccessAsync(int id, string link);
        Task<IList<BatchReportAccountBalanceDomain>> ListAsync();
        Task<IList<BatchReportAccountBalanceDomain>> ListPendingAsync();
    }
}
