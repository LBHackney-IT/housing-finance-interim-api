namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The operating balance entity.
    /// </summary>
    public class Tenancy
    {
        /// <summary>
        /// Gets or sets the tenancy agreement reference.
        /// </summary>
        public string TenancyAgreementRef { get; set; }

        /// <summary>
        /// Gets or sets the property reference.
        /// </summary>
        public string PropertyRef { get; set; }

        /// <summary>
        /// Gets or sets the household reference.
        /// </summary>
        public string HouseholdRef { get; set; }

        /// <summary>
        /// Gets or sets the rent account number.
        /// </summary>
        public string RentAccount { get; set; }

        /// <summary>
        /// Gets or sets the member title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the member forename.
        /// </summary>
        public string Forename { get; set; }

        /// <summary>
        /// Gets or sets the member surname.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the telephone.
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the current balance.
        /// </summary>
        public decimal CurrentBalance { get; set; }

        public decimal Rent { get; set; }

        public decimal Service { get; set; }

        public decimal OtherCharge { get; set; }

        /// <summary>
        /// Gets or sets the tenure.
        /// </summary>
        public string Tenure { get; set; }

        /// <summary>
        /// Gets or sets the rent group.
        /// </summary>
        public string RentGroup { get; set; }

        /// <summary>
        /// Gets or sets the address 1.
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2.
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the address 3.
        /// </summary>
        public string Address3 { get; set; }

        /// <summary>
        /// Gets or sets the address 4.
        /// </summary>
        public string Address4 { get; set; }

        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        public string PostCode { get; set; }
    }

}
