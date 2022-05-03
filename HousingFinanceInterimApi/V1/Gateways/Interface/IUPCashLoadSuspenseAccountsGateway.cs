using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IUPCashLoadSuspenseAccountsGateway
    {
        Task CreateBulkAsync(IList<CashSuspenseTransactionAuxDomain> cashSuspenseDomain);
        Task ClearCashSuspenseTransactionsAuxAuxiliary();
        Task<IList<CashSuspenseTransactionAuxDomain>> GetCashSuspenseTransactions();
        Task LoadCashSuspenseTransactions();
    }
}
