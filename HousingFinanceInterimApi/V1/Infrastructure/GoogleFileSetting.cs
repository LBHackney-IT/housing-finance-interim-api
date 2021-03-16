using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The Google folder config table.
    /// </summary>
    [Table("GoogleFileSetting")]
    public class GoogleFileSetting
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the google folder/file identifier.
        /// </summary>
        public string GoogleIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the type of the file.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

    }

}
