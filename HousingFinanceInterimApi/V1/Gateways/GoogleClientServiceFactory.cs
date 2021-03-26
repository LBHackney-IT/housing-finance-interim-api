using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Util.Store;
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
        public async Task<IGoogleClientService> CreateGoogleClientServiceForUserAsync(string authCode)
        {
            // TODO make use of data store after testing
            var dataStore = new GoogleEntityDataStore(_context);

            using IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = _options.ClientId,
                        ClientSecret = _options.ClientSecret
                    },
                    Scopes = _options.Scopes,

                    // TODO use entityDataStore
                    DataStore = new FileDataStore("GoogleTokens")
                });

            TokenResponse token = await flow.ExchangeCodeForTokenAsync("USER_ID",
                authCode, _options.RedirectUri, CancellationToken.None);
            UserCredential credential = new UserCredential(flow, Environment.UserName, token);

            // Create service initializer
            BaseClientService.Initializer initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _options.ApplicationName
            };

            return new GoogleClientService(_logger, initializer);
        }

        /// <summary>
        /// Creates the google client service for API key asynchronous.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <returns>
        /// A Google Client Service instance.
        /// </returns>
        public IGoogleClientService CreateGoogleClientServiceForApiKey(string apiKey)
        {
            BaseClientService.Initializer initializer = new BaseClientService.Initializer
            {
                ApplicationName = _options.ApplicationName,
                ApiKey = apiKey
            };

            return new GoogleClientService(_logger, initializer);
        }

    }

}
