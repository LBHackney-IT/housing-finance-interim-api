using System;

namespace HousingFinanceInterimApi.V1.Domain
{

    /// <summary>
    /// The Google file setting domain object.
    /// </summary>
    public class GoogleFileSettingDomain
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the file year.
        /// </summary>
        public int FileYear { get; set; }

        /// <summary>
        /// Gets or sets the google folder/file identifier.
        /// </summary>
        public string GoogleIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the type of the file.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive => !EndDate.HasValue || EndDate.Value > DateTimeOffset.UtcNow;

    }

}
