using AutoMapper;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Domain.AutoMaps
{

    /// <summary>
    /// The rent breakdown mapping profile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class OtherHRAMappingProfile : Profile
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="OtherHRAMappingProfile"/> class.
        /// </summary>
        public OtherHRAMappingProfile()
        {
            CreateMap<OtherHRA, OtherHRADomain>();
            CreateMap<OtherHRADomain, OtherHRA>();
        }

    }

}
