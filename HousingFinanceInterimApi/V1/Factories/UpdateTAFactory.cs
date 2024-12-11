using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Domain;
using System;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class UpdateTAFactory
    {
        public static UpdateTADomain ToDomain(this UpdateTARequest request)
        {
            if (request == null) return null;
            var defaultDate = new DateTime(1900, 01, 01);
            if (request.TenureEndDate == null || request.TenureEndDate > DateTime.UtcNow || request.TenureEndDate == defaultDate)
            {
                return new UpdateTADomain
                {
                    TenureEndDate = request.TenureEndDate,
                    IsTerminated = false,
                    IsPresent = true
                };
            }
            else
            {
                return new UpdateTADomain
                {
                    TenureEndDate = request.TenureEndDate,
                    IsTerminated = true,
                    IsPresent = false
                };
            };

        }
    }
}
