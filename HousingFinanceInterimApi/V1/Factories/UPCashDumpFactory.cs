using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{

    /// <summary>
    /// The UP cash dump factory.
    /// </summary>
    public static class UPCashDumpFactory
    {
        public static UPCashDumpDomain ToDomain(this UPCashDump cashDump)
        {
            if (cashDump == null)
                return null;

            return new UPCashDumpDomain
            {
                FullText = cashDump.FullText,
                Id = cashDump.Id,
                Timestamp = cashDump.Timestamp,
                UPCashDumpFileNameId = cashDump.UPCashDumpFileNameId
            };
        }

        public static List<UPCashDumpDomain> ToDomain(
            this ICollection<UPCashDump> cashDump)
        {
            return cashDump?.Select(d => d.ToDomain()).ToList();
        }
    }

}
