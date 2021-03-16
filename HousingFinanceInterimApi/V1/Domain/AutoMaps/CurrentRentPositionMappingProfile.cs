using AutoMapper;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Domain.AutoMaps
{

    /// <summary>
    /// The current rent position mapping profile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class CurrentRentPositionMappingProfile : Profile
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentRentPositionMappingProfile"/> class.
        /// </summary>
        public CurrentRentPositionMappingProfile()
        {
            CreateMap<CurrentRentPosition, CurrentRentPositionDomain>();
            CreateMap<CurrentRentPositionDomain, CurrentRentPosition>();
        }

    }

}
