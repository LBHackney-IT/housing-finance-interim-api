using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The Google file setting gateway interface.
    /// </summary>
    public interface IGoogleFileSettingGateway
    {

        /// <summary>
        /// Lists the google file settings asynchronous.
        /// </summary>
        /// <returns>The list of Google file settings.</returns>
        public Task<IList<GoogleFileSetting>> ListAsync();

    }

}
