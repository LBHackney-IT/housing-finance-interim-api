namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The operating balance entity.
    /// </summary>
    public class Payment
    {

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        public int YearNo { get; set; }

        /// <summary>
        /// Gets or sets the period (week number or month number).
        /// </summary>
        public int PeriodNo { get; set; }

        /// <summary>
        /// Gets or sets the amount paid.
        /// </summary>
        public decimal Amount { get; set; }
    }

}
