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
    public static class ServiceChargePaymentsReceivedFactory
    {

        /// <summary>
        /// Converts the given current rent position domain objects to entity.
        /// </summary>
        /// <param name="autoMapper">The automatic mapper.</param>
        /// <param name="domainObjects">The domain objects.</param>
        /// <returns>The current rent position entities.</returns>
        public static IList<ServiceChargePaymentsReceived> ToEntity(IMapper autoMapper, IList<ServiceChargePaymentsReceivedDomain> domainObjects)
            => domainObjects.Select(autoMapper.Map<ServiceChargePaymentsReceived>).ToList();

    }

}
