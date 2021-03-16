using AutoMapper;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{

    /// <summary>
    /// The rent breakdown factory class.
    /// </summary>
    public static class RentBreakdownFactory
    {

        /// <summary>
        /// Converts the given domain objects to entity.
        /// </summary>
        /// <param name="autoMapper">The automatic mapper.</param>
        /// <param name="domainObjects">The domain objects.</param>
        /// <returns>The list of rent breakdown entity.</returns>
        public static IList<RentBreakdown> ToEntity(IMapper autoMapper, IList<RentBreakdownDomain> domainObjects)
            => domainObjects.Select(autoMapper.Map<RentBreakdown>).ToList();

    }

}
