using AutoMapper;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Domain.AutoMaps
{

    /// <summary>
    /// The rent breakdown mapping profile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class ServiceChargePaymentsReceivedMappingProfile : Profile
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceChargePaymentsReceivedMappingProfile"/> class.
        /// </summary>
        public ServiceChargePaymentsReceivedMappingProfile()
        {
            CreateMap<ServiceChargePaymentsReceived, ServiceChargePaymentsReceivedDomain>();
            CreateMap<ServiceChargePaymentsReceivedDomain, ServiceChargePaymentsReceived>();
        }

    }

}
