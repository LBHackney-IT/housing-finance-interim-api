using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IDirectDebitGateway
    {
        public Task<List<DirectDebitAuxDomain>> CreateBulkAsync(IList<DirectDebitAuxDomain> directDebitsDomain);

        public Task ClearDirectDebitAuxiliary();

        public Task LoadDirectDebit(long batchLogId);

        public Task LoadDirectDebitHistory(DateTime? processingDate);
    }
}
