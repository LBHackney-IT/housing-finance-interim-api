using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Gateways.Options;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HousingFinanceInterimApi.V1.Gateways
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
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleClientServiceFactory" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="context">The database context.</param>
        public GoogleClientServiceFactory(ILogger logger, IOptions<GoogleClientServiceOptions> options,
            DatabaseContext context)
        {
            _logger = logger;
            _options = options.Value;
            _context = context;
        }

        /// <summary>
        /// Creates the google client service implementation for the given google user ID asynchronous.
        /// </summary>
        /// <param name="authCode">The authentication code.</param>
        /// <returns>
        /// A Google Client Service instance.
        /// </returns>
        public IGoogleClientService CreateGoogleClientServiceForUser(string authCode)
            => new GoogleClientService(_logger, _options, new GoogleEntityDataStore(_context), authCode);

    }

}
