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
        public decimal TotalCharged { get; set; }

        /// <summary>
        /// Gets or sets the total rent paid.
        /// </summary>
        public decimal TotalPaid { get; set; }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        public decimal TotalBalance { get; set; }

        /// <summary>
        /// Gets or sets the total rent YTD.
        /// </summary>
        public decimal ChargedYTD { get; set; }

        /// <summary>
        /// Gets or sets the total rent YTD.
        /// </summary>
        public decimal PaidYTD { get; set; }

        /// <summary>
        /// Gets or sets the arrears YTD.
        /// </summary>
        public decimal ArrearsYTD { get; set; }

    }

}
