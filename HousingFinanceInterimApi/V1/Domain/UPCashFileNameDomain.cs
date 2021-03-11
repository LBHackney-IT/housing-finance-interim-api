using System;

namespace HousingFinanceInterimApi.V1.Domain
{

    /// <summary>
    /// The UP Cash Dump file name domain object.
    /// </summary>
    public class UPCashFileNameDomain
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is success.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

    }

}
