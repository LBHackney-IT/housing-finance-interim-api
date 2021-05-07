using System;

namespace HousingFinanceInterimApi.V1.Domain
{

    /// <summary>
    /// The UP cash dump domain object.
    /// </summary>
    public class UPHousingCashDumpDomain
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets up cash dump file name identifier.
        /// </summary>
        public long UPHousingCashDumpFileNameId { get; set; }

        /// <summary>
        /// Gets or sets the full text.
        /// </summary>
        public string FullText { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

    }

}
