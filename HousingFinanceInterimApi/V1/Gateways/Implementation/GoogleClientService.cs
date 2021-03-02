using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using HousingFinanceInterimApi.V1.Gateways.Implementation.Options;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        private readonly ILogger _logger;

        #region Drive service

        /// <summary>
        /// The drive service backing variable
        /// </summary>
        private DriveService _driveServiceBacking;

        /// <summary>
        /// Gets the drive service.
        /// </summary>
        private DriveService _driveService => _driveServiceBacking ??= new DriveService(_initializer);

        #endregion

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
        /// <param name="entityDataStore">The entity data store.</param>
        /// <param name="authorizationCode">The authorization code.</param>
        public GoogleClientService(ILogger logger, GoogleClientServiceOptions options, IDataStore entityDataStore,
            string authorizationCode)
        {
            _logger = logger;

            using IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = options.ClientId, ClientSecret = options.ClientSecret
                    },
                    Scopes = options.Scopes,
                    // TODO use entityDataStore
                    DataStore = new FileDataStore("GoogleTokens")
                });

            TokenResponse token = Task.Run(() => flow.ExchangeCodeForTokenAsync("USER_ID",
                    authorizationCode, options.RedirectUri, CancellationToken.None))
                .GetAwaiter()
                .GetResult();
            UserCredential credential = new UserCredential(flow, Environment.UserName, token);

            // Create service initializer
            _initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = credential, ApplicationName = options.ApplicationName
            };
        }

        #region Google Drive

        /// <summary>
        /// Ensures the user's shared Google folder exists.
        /// </summary>
        /// <param name="userEmail">The user email.</param>
        /// <param name="userGoogleId">The user google identifier.</param>
        /// <returns>
        /// A bool determining the success of the method.
        /// </returns>
        public async Task<bool> EnsureUserFolderExistsAsync(string userEmail, string userGoogleId)
        {
            FilesResource.CreateRequest createRequest = _driveService.Files.Create(new File
            {
                Name = userEmail,
                MimeType = "application/vnd.google-apps.folder",
                PermissionIds = new List<string>
                {
                    userGoogleId
                },
                Parents = new List<string>
                {
                    // TODO parent containing folder
                    ""
                }
            });

            File createResult = await createRequest.ExecuteAsync();
            string folderId = createResult.Id;

            // TODO save configuration

            return !string.IsNullOrWhiteSpace(folderId);
        }

        #endregion

        #region Google Sheets

        public Task ReadSheetToJsonAsync(string spreadSheetId, string sheetName, string sheetRange, string outputFileName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

    }

}
