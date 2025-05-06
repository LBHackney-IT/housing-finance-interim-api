using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface INightlyProcessLogGateway
    {
        Task<IList<NightlyProcessLog>> GetByDateCreatedAsync(DateTime createdDate);
    }
}
