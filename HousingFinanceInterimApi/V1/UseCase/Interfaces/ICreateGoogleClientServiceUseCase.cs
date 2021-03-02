using HousingFinanceInterimApi.V1.Gateways.Interface;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The create <see cref="IGoogleClientService"/> use case.
    /// </summary>
    public interface ICreateGoogleClientServiceUseCase
    {

        /// <summary>
        /// Executes the use case for the given authentication code.
        /// </summary>
        /// <param name="authCode">The authentication code.</param>
        /// <returns>An instance of <see cref="IGoogleClientService"/></returns>
        public IGoogleClientService Execute(string authCode);

    }

}
