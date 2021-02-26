using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using HousingFinanceInterimApi.V1.Gateways.Implementation.Options;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Implementation
{

    /// <summary>
    /// The google client service implementation.
    /// </summary>
    /// <seealso cref="IGoogleClientService" />
    public class GoogleClientService : IGoogleClientService
    {

        #region Private

        /// <summary>
        /// The service initializer
        /// </summary>
        private readonly BaseClientService.Initializer _initializer;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GoogleClientService> _logger;

        #region Sheets service

        /// <summary>
        /// The sheets service backing variable
        /// </summary>
        private SheetsService _sheetsServiceBacking;

        /// <summary>
        /// Gets the sheets service.
        /// </summary>
        private SheetsService _sheetsService => _sheetsServiceBacking ??= new SheetsService(_initializer);

        #endregion

        #endregion

        #region Public

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleClientService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The client options.</param>
        public GoogleClientService(ILogger<GoogleClientService> logger, IOptions<GoogleClientServiceOptions> options)
        {
            _logger = logger;

            // The file googletoken.json stores the user's access and refresh tokens, and is created
            // automatically when the authorization flow completes for the first time.
            const string CREDENTIAL_PATH = "googletoken.json";

            // Create the client secrets
            ClientSecrets clientSecrets = new ClientSecrets
            {
                ClientId = options.Value.ClientId, ClientSecret = options.Value.ClientSecret
            };

            // Create credential
            UserCredential userCredential = Task.Run(() => GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets,
                    options.Value.Scopes, "user", CancellationToken.None, new FileDataStore(CREDENTIAL_PATH, true)))
                .GetAwaiter()
                .GetResult();
            logger.LogInformation("Google credential file saved to: " + CREDENTIAL_PATH);

            // Create service initializer
            _initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = userCredential, ApplicationName = options.Value.ApplicationName
            };
        }

        public Task ReadSheetToJsonAsync(string spreadSheetId, string sheetName, string sheetRange, string outputFileName)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
