using AutoMapper;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{

    /// <summary>
    /// The current rent position factory.
    /// </summary>
    public static class LeaseholdAccountsFactory
    {

        /// <summary>
        /// Converts the given leasehold accounts domain objects to entity.
        /// </summary>
        /// <param name="autoMapper">The automatic mapper.</param>
        /// <param name="domainObjects">The domain objects.</param>
        /// <returns>The leasehold accounts entities.</returns>
        public static IList<LeaseholdAccount> ToEntity(IMapper autoMapper, IList<LeaseholdAccountDomain> domainObjects)
            => domainObjects.Select(autoMapper.Map<LeaseholdAccount>).ToList();

    }

}
