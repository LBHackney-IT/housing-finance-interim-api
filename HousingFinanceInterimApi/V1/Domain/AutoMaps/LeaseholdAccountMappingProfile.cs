using AutoMapper;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Domain.AutoMaps
{

    /// <summary>
    /// The rent breakdown mapping profile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class LeaseholdAccountMappingProfile : Profile
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaseholdAccountMappingProfile"/> class.
        /// </summary>
        public LeaseholdAccountMappingProfile()
        {
            CreateMap<LeaseholdAccount, LeaseholdAccountDomain>();
            CreateMap<LeaseholdAccountDomain, LeaseholdAccount>();
        }

    }

}
