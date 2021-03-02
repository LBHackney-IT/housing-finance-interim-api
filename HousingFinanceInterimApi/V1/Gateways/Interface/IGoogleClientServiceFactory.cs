namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The google client service factory interface.
    /// </summary>
    public interface IGoogleClientServiceFactory
    {

        /// <summary>
        /// Creates the google client service for the given google user ID asynchronous.
        /// </summary>
        /// <param name="authCode">The authentication code.</param>
        /// <returns>
        /// A Google Client Service instance.
        /// </returns>
        public IGoogleClientService CreateGoogleClientServiceForUser(string authCode);

    }

}
