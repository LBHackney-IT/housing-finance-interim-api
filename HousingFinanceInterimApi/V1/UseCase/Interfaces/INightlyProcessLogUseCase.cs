using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface INightlyProcessLogUseCase
    {
        Task<List<NightlyProcessLogResponse>> ExecuteAsync(DateTime createdDate);
    }
}
