using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IChargesGateway
    {
        public Task<List<ChargesAuxDomain>> CreateBulkAsync(IList<ChargesAuxDomain> directDebitsDomain);

        public Task ClearChargesAuxiliary();

        public Task LoadCharges();

        public Task LoadChargesHistory(DateTime? processingDate);
    }
}
