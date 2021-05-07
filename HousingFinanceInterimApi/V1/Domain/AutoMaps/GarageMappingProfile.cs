using AutoMapper;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Domain.AutoMaps
{

    /// <summary>
    /// The rent breakdown mapping profile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class GarageMappingProfile : Profile
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="GarageMappingProfile"/> class.
        /// </summary>
        public GarageMappingProfile()
        {
            CreateMap<Garage, GarageDomain>();
            CreateMap<GarageDomain, Garage>();
        }

    }

}
