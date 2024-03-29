using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IBatchReportGateway
    {
        Task<BatchReportDomain> CreateAsync(BatchReportDomain batchReportAccountBalanceDomain);
        Task<bool> SetStatusAsync(int id, string link, bool isSuccess);
        Task<IList<BatchReportDomain>> ListAsync(string reportName);
        Task<IList<BatchReportDomain>> ListPendingAsync();
    }
}
