using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface ISuspenseAccountsGateway
    {
        Task CreateBulkAsync(IList<SuspenseTransactionAuxDomain> suspenseTransactionsAuxDomain, string type);
        Task ClearSuspenseTransactionsAuxAuxiliary();
        Task<IList<SuspenseTransactionAuxDomain>> GetCashSuspenseTransactions();
        Task LoadCashSuspenseTransactions();
        Task<IList<SuspenseTransactionAuxDomain>> GetHousingBenefitSuspenseTransactions();
        Task LoadHousingBenefitSuspenseTransactions();
    }
}
