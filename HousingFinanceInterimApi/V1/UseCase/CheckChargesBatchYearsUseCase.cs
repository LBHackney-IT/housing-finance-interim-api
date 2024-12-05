using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using Google.Apis.Drive.v3.Data;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class CheckChargesBatchYearsUseCase : ICheckChargesBatchYearsUseCase
    {
        private readonly IChargesBatchYearsGateway _chargesBatchYearsGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private readonly string _chargesBatchYears = Environment.GetEnvironmentVariable("CHARGES_BATCH_YEARS");

        private const string ChargesLabel = "Charges";

        public CheckChargesBatchYearsUseCase(
            IChargesBatchYearsGateway chargesBatchYearsGateway)
        {
            _chargesBatchYearsGateway = chargesBatchYearsGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            var existDate = await _chargesBatchYearsGateway.ExistDataForToday().ConfigureAwait(false);
            if (!existDate)
            {
                var chargesBatchYears = _chargesBatchYears.Split(';').Select(Int32.Parse).ToList();
                foreach (var year in chargesBatchYears)
                {
                    await _chargesBatchYearsGateway.CreateAsync(year).ConfigureAwait(false);
                }
            }

            var pendingYear = await _chargesBatchYearsGateway.GetPendingYear().ConfigureAwait(false);
            if (pendingYear == null)
            {
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
            }

            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }

    }
}
