using System;
using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface ILoadChargesTransactionsUseCase
    {
        public Task<StepResponse> ExecuteAsync();

        public Task<StepResponse> ExecuteOnDemandAsync(DateTime startDate, DateTime endDate);
    }

}
