using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{

    /// <summary>
    /// The Google file setting factory.
    /// </summary>
    public static class GoogleFileSettingFactory
    {

        /// <summary>
        /// Converts the given list of Google file settings to domain objects.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>The Google file settings domain objects.</returns>
        public static IList<GoogleFileSettingDomain> ToDomain(IList<GoogleFileSetting> settings)
            => settings.Select(item => new GoogleFileSettingDomain
                {
                    Id = item.Id,
                    GoogleFolderId = item.GoogleFolderId,
                    FileType = item.FileType,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate
                })
                .ToList();

    }

}
