using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class ChargesBatchYearsFactory
    {
        public static ChargesBatchYearDomain ToDomain(this ChargesBatchYear chargesBatchYear)
        {
            if (chargesBatchYear == null)
                return null;

            return new ChargesBatchYearDomain
            {
                Id = chargesBatchYear.Id,
                ProcessingDate = chargesBatchYear.ProcessingDate,
                Year = chargesBatchYear.Year,
                IsRead = chargesBatchYear.IsRead
            };
        }

        public static List<ChargesBatchYearDomain> ToDomain(
            this ICollection<ChargesBatchYear> chargesBatchYear)
        {
            return chargesBatchYear?.Select(b => b.ToDomain()).ToList();
        }
    }
}
