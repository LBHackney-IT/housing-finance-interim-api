using HousingFinanceInterimApi.V1.Gateways.Implementation.Options;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HousingFinanceInterimApi.V1.Gateways.Implementation
{

    /// <summary>
    /// The Google client service factory.
    /// </summary>
    /// <seealso cref="IGoogleClientServiceFactory" />
    public class GoogleClientServiceFactory : IGoogleClientServiceFactory
    {

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The Google client service options
        /// </summary>
        private readonly GoogleClientServiceOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleClientServiceFactory" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        public GoogleClientServiceFactory(ILogger logger, IOptions<GoogleClientServiceOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        /// <summary>
        /// Creates the google client service implementation for the given google user ID asynchronous.
        /// </summary>
        /// <param name="googleId">The google identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>
        /// A Google Client Service instance.
        /// </returns>
        public IGoogleClientService CreateGoogleClientServiceForUser(string googleId, string accessToken,
            string refreshToken)
            => new GoogleClientService(_logger, _options, googleId, accessToken, refreshToken);

    }

}
