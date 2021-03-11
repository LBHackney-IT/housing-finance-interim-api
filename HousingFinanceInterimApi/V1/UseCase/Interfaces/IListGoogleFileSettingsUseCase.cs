using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The list google file settings use case.
    /// </summary>
    public interface IListGoogleFileSettingsUseCase
    {

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>The list of Google file settings.</returns>
        public Task<IList<GoogleFileSettingDomain>> Execute();

    }

}
