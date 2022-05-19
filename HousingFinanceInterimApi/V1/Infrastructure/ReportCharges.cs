using System;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    public class ReportCharges
    {

        /// <summary>
        /// Gets or sets the rent group.
        /// </summary>
        public string RentAccount { get; set; }

        /// <summary>
        /// Gets or sets the total rent due.
        /// </summary>
        public DateTime EndOfTenancy { get; set; }

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
