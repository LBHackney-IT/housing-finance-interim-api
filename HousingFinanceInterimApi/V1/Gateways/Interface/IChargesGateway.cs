using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IChargesGateway
    {
        public Task CreateBulkAsync(IList<ChargesAuxDomain> chargesAuxDomain, string rentGroup, int year);

        public Task ClearChargesAuxiliary();

        public Task LoadCharges();

        public Task LoadChargesHistory(int processingYear);
    }
}
