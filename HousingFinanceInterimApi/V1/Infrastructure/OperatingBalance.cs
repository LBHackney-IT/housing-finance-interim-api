namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The operating balance entity.
    /// </summary>
    public class OperatingBalance
    {

        /// <summary>
        /// Gets or sets the rent group.
        /// </summary>
        public string RentGroup { get; set; }

        /// <summary>
        /// Gets or sets the total rent due.
        /// </summary>
        public decimal TotalRentDue { get; set; }

        /// <summary>
        /// Gets or sets the total rent paid.
        /// </summary>
        public decimal TotalRentPaid { get; set; }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        public decimal Balance { get; set; }

    }

}
