using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class UpdateTAFactory
    {
        public static UpdateTADomain ToDomain(this UpdateTARequest request)
        {
            if (request == null) return null;
            bool isTerminated;
            bool isPresent;
            var convertToString = request.TenureEndDate.ToString();
            if (request.TenureEndDate == null || request.TenureEndDate > DateTime.UtcNow || convertToString == "01/01/1900 00:00:00")
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
