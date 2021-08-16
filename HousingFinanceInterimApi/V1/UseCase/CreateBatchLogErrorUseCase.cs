using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class CreateBatchLogErrorUseCase : ICreateBatchLogErrorUseCase
    {
        private readonly IBatchLogErrorGateway _gateway;

        public CreateBatchLogErrorUseCase(IBatchLogErrorGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<BatchLogErrorDomain> ExecuteAsync(long batchId, string type, string message)
            => BatchLogErrorFactory.ToDomain(await _gateway.CreateAsync(batchId, type, message).ConfigureAwait(false));
    }
}
