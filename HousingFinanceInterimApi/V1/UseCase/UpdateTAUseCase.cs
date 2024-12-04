using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Infrastructure;
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
            LoggingHandler.LogInfo($"eot value is {request.TenureEndDate}");
            var domain = request.ToDomain();
            LoggingHandler.LogInfo($"eot value is {domain.TenureEndDate}, present:{domain.IsPresent}, terminated:{domain.IsTerminated}");
            await _gateway.UpdateTADetails(tagRef, domain).ConfigureAwait(false);
        }

    }
}
