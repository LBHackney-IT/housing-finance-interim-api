using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase
{

    /// <summary>
    /// The list Google file setting use case implementation.
    /// </summary>
    /// <seealso cref="IListGoogleFileSettingsUseCase" />
    public class ListGoogleFileSettingsUseCase : IListGoogleFileSettingsUseCase
    {

        /// <summary>
        /// The google file setting gateway
        /// </summary>
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListGoogleFileSettingsUseCase"/> class.
        /// </summary>
        /// <param name="settingGateway">The setting gateway.</param>
        public ListGoogleFileSettingsUseCase(IGoogleFileSettingGateway settingGateway)
        {
            _googleFileSettingGateway = settingGateway;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>
        /// The list of Google file settings.
        /// </returns>
        public async Task<IList<GoogleFileSettingDomain>> Execute()
        {
            var settings = await _googleFileSettingGateway.ListAsync().ConfigureAwait(false);

            return GoogleFileSettingFactory.ToDomain(settings);
        }

    }

}
