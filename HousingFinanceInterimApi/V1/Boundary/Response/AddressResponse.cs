namespace HousingFinanceInterimApi.V1.Boundary.Response
{

    /// <summary>
    /// The address response model.
    /// </summary>
    public class AddressResponse
    {

        /// <summary>
        /// Gets or sets the address line1.
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the address line2.
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the town.
        /// </summary>
        public string Town { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        public string Postcode { get; set; }

    }

}
