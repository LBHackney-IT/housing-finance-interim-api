using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The error log entity class.
    /// </summary>
    [Table("ErrorLog")]
    public class ErrorLog
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        public int Id { get; set; }

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
        public DateTimeOffset Timestamp { get; private set; }

    }

}
