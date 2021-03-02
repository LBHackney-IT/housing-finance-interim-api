using System.Collections.Generic;

namespace HousingFinanceInterimApi.V1.Gateways.Options
{

    /// <summary>
    /// The Google client service implementation options.
    /// </summary>
    public class GoogleClientServiceOptions
    {

        /// <summary>
        /// Gets or sets the credentials path.
        /// </summary>
        public string CredentialsPath { get; set; }

        /// <summary>
        /// Gets or sets the application's permission scopes.
        /// </summary>
        public IList<string> Scopes { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string ClientSecret { get; set; }

    }

}
