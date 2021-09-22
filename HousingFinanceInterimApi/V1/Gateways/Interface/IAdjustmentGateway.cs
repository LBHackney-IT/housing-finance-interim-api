using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public interface IAdjustmentGateway
    {
        public Task CreateBulkAsync(IList<AdjustmentDomain> adjustmentDomain);
        public Task LoadTransactions();
    }
}
