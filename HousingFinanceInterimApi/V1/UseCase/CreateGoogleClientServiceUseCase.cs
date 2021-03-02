using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;

namespace HousingFinanceInterimApi.V1.UseCase
{

    /// <summary>
    /// The create Google client service use case implementation.
    /// </summary>
    /// <seealso cref="ICreateGoogleClientServiceUseCase" />
    public class CreateGoogleClientServiceUseCase : ICreateGoogleClientServiceUseCase
    {

        /// <summary>
        /// The google client service factory
        /// </summary>
        private readonly IGoogleClientServiceFactory _googleClientServiceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateGoogleClientServiceUseCase"/> class.
        /// </summary>
        /// <param name="googleClientServiceFactory">The google client service factory.</param>
        public CreateGoogleClientServiceUseCase(IGoogleClientServiceFactory googleClientServiceFactory)
        {
            _googleClientServiceFactory = googleClientServiceFactory;
        }

        /// <summary>
        /// Executes the use case for the given authentication code.
        /// </summary>
        /// <param name="authCode">The authentication code.</param>
        /// <returns>
        /// An instance of <see cref="IGoogleClientService" />
        /// </returns>
        public IGoogleClientService Execute(string authCode)
            => _googleClientServiceFactory.CreateGoogleClientServiceForUser(authCode);

    }

}
