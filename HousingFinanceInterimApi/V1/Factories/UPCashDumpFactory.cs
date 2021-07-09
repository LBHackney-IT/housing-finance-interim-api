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

        /// <summary>
        /// Converts the given cash dumps to domain objects.
        /// </summary>
        /// <param name="cashDumps">The cash dumps.</param>
        /// <returns>The UP cash dump domain objects.</returns>
        public static IList<UPCashDumpDomain> ToDomain(IList<UPCashDump> cashDumps)
            => cashDumps.Select(item => new UPCashDumpDomain
            {
                FullText = item.FullText,
                Id = item.Id,
                Timestamp = item.Timestamp,
                UPCashDumpFileNameId = item.UPCashDumpFileNameId
            })
                .ToList();

    }

}
