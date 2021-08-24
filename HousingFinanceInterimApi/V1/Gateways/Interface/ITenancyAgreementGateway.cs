using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface ITenancyAgreementGateway
    {
        public Task<List<TenancyAgreementAuxDomain>> CreateBulkAsync(IList<TenancyAgreementAuxDomain> directDebitsDomain);

        public Task ClearTenancyAgreementAuxiliary();

        public Task RefreshTenancyAgreement();
    }
}
