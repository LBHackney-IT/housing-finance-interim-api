using HousingFinanceInterimApi.V1.Boundary.Request;
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
            var convertToString = request.TenureEndDate.ToString();
            if (request.TenureEndDate == null || request.TenureEndDate > DateTime.UtcNow)
            {
                request.IsTerminated = false;
                request.IsPresent = true;
            }
            else
            {
                request.IsTerminated = true;
                request.IsPresent = false;
            }
            await _gateway.UpdateTADetails(tagRef, request).ConfigureAwait(false);
        }

    }
}
