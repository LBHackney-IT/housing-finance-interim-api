using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class UpdateTAUseCase : IUpdateTAUseCase
    {
        private readonly IUpdateTAGateway _gateway;

        public UpdateTAUseCase(IUpdateTAGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task ExecuteAsync(string tagRef, UpdateTARequest request)
        {
            var domain = request.ToDomain();
            await _gateway.UpdateTADetails(tagRef, domain).ConfigureAwait(false);
        }

    }
}
