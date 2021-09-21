using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{

    /// <summary>
    /// The UP cash dump factory.
    /// </summary>
    public static class UPHousingCashDumpFactory
    {
        public static UPHousingCashDumpDomain ToDomain(this UPHousingCashDump housingCashDump)
        {
            if (housingCashDump == null)
                return null;

            return new UPHousingCashDumpDomain
            {
                FullText = housingCashDump.FullText,
                Id = housingCashDump.Id,
                Timestamp = housingCashDump.Timestamp,
                UPHousingCashDumpFileNameId = housingCashDump.UPHousingCashDumpFileNameId
            };
        }

        public static List<UPHousingCashDumpDomain> ToDomain(
            this ICollection<UPHousingCashDump> housingCashDump)
        {
            return housingCashDump?.Select(d => d.ToDomain()).ToList();
        }
    }

}
