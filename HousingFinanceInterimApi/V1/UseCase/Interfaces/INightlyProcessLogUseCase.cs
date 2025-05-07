using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface INightlyProcessLogUseCase
    {
        Task<StepResponse> ExecuteAsync();

        Task<IList<NightlyProcessLogResponse>> ExecuteAsync(DateTime createdDate);
    }
}
