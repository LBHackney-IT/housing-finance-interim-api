using System;

namespace HousingFinanceInterimApi.V1.Domain
{

    /// <summary>
    /// The error log domain object.
    /// </summary>
    public class ErrorLogDomain
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the row identifier.
        /// </summary>
        public string RowId { get; set; }

        /// <summary>
        /// Gets or sets the user friendly error.
        /// </summary>
        public string UserFriendlyError { get; set; }

        /// <summary>
        /// Gets or sets the application error.
        /// </summary>
        public string ApplicationError { get; set; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }

    }

}
