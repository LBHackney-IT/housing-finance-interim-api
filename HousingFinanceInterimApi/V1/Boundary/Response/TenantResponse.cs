namespace HousingFinanceInterimApi.V1.Boundary.Response
{

    /// <summary>
    /// The tenant response model.
    /// </summary>
    public class TenantResponse
    {

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public AddressResponse Address { get; set; }

        /// <summary>
        /// Gets or sets the contact details.
        /// </summary>
        public ContactDetailsResponse ContactDetails { get; set; }

    }

}
