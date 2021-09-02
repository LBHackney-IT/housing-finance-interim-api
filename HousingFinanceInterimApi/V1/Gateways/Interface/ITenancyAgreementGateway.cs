using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface ITenancyAgreementGateway
    {
        public Task CreateBulkAsync(IList<TenancyAgreementAuxDomain> directDebitsDomain);

        public Task ClearTenancyAgreementAuxiliary();

        public Task RefreshTenancyAgreement();
    }
}
