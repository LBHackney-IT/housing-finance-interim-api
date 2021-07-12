using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The error log gateway.
    /// </summary>
    public interface IErrorLogGateway
    {

        /// <summary>
        /// Performs an error log asynchronous.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="rowId">The row identifier.</param>
        /// <param name="userFriendlyError">The user friendly error.</param>
        /// <param name="applicationError">The application error.</param>
        /// <returns>The created <see cref="ErrorLog"/> instance.</returns>
        public Task<ErrorLog> LogAsync(string tableName, string rowId, string userFriendlyError, string applicationError);

    }

}
