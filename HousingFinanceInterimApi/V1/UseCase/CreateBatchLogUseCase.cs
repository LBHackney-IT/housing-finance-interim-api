using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class CreateBatchLogUseCase : ICreateBatchLogUseCase
    {
        private readonly IBatchLogGateway _gateway;

        public CreateBatchLogUseCase(IBatchLogGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<BatchLogDomain> ExecuteAsync(string type)
            => BatchLogFactory.ToDomain(await _gateway.CreateAsync(type).ConfigureAwait(false));

    }
}
