using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IActionDiaryGateway
    {
        public Task<List<ActionDiaryAuxDomain>> CreateBulkAsync(IList<ActionDiaryAuxDomain> actionsDiaryDomain);

        public Task ClearActionDiaryAuxiliary();

        public Task LoadActionDiary();

        //public Task LoadActionDiaryHistory(DateTime? processingDate);
    }
}
