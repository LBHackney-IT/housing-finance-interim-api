using System;

namespace HousingFinanceInterimApi.V1.Boundary.Response
{

    /// <summary>
    /// The operating balance entry response model.
    /// </summary>
    public class OperatingBalanceEntryResponse
    {

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the balance date.
        /// </summary>
        public DateTimeOffset BalanceDate { get; set; }

        /// <summary>
        /// Gets or sets the arrears.
        /// </summary>
        public decimal Arrears { get; set; }

        /// <summary>
        /// Gets or sets the CSV download link.
        /// </summary>
        public string CSVDownloadLink { get; set; }

    }

}
