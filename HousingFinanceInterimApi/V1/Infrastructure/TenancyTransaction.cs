using System;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The operating balance entity.
    /// </summary>
    public class TenancyTransaction
    {

        /// <summary>
        /// Gets or sets the week beginning.
        /// </summary>
        public DateTime WeekBeginning { get; set; }

        /// <summary>
        /// Gets or sets the total charged.
        /// </summary>
        public decimal TotalCharged { get; set; }

        /// <summary>
        /// Gets or sets the total paid.
        /// </summary>
        public decimal TotalPaid { get; set; }

        /// <summary>
        /// Gets or sets the total housing benefit paid.
        /// </summary>
        public decimal TotalHB { get; set; }

        /// <summary>
        /// Gets or sets the week balance.
        /// </summary>
        public decimal WeekBalance { get; set; }
    }

}
