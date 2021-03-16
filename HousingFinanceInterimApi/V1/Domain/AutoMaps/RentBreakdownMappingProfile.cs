using AutoMapper;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Domain.AutoMaps
{

    /// <summary>
    /// The rent breakdown mapping profile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class RentBreakdownMappingProfile : Profile
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RentBreakdownMappingProfile"/> class.
        /// </summary>
        public RentBreakdownMappingProfile()
        {
            CreateMap<RentBreakdown, RentBreakdownDomain>();
            CreateMap<RentBreakdownDomain, RentBreakdown>();
        }

    }

}
