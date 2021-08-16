using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class SetBatchLogSuccessUseCase : ISetBatchLogSuccessUseCase
    {
        private readonly IBatchLogGateway _gateway;


        public SetBatchLogSuccessUseCase(IBatchLogGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task ExecuteAsync(long batchId)
        {
            await _gateway.SetToSuccessAsync(batchId).ConfigureAwait(false);
        }
    }
}
