using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{

    /// <summary>
    /// The error log factory.
    /// </summary>
    public static class ErrorLogFactory
    {

        /// <summary>
        /// Converts the given input to domain.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns>The <see cref="ErrorLogDomain"/> instance.</returns>
        public static ErrorLogDomain ToDomain(ErrorLog log) => new ErrorLogDomain
        {
            TableName = log.TableName,
            RowId = log.RowId,
            Id = log.Id,
            ApplicationError = log.ApplicationError,
            Timestamp = log.Timestamp,
            UserFriendlyError = log.UserFriendlyError
        };

    }

}
