using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The error log gateway implementation.
    /// </summary>
    /// <seealso cref="IErrorLogGateway" />
    public class ErrorLogGateway : IErrorLogGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ErrorLogGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Performs an error log asynchronous.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="rowId">The row identifier.</param>
        /// <param name="userFriendlyError">The user friendly error.</param>
        /// <param name="applicationError">The application error.</param>
        /// <returns>
        /// The created <see cref="ErrorLog" /> instance.
        /// </returns>
        public async Task<ErrorLog> LogAsync(string tableName, string rowId, string userFriendlyError,
            string applicationError)
        {
            ErrorLog log = new ErrorLog
            {
                TableName = tableName,
                RowId = rowId,
                UserFriendlyError = userFriendlyError,
                ApplicationError = applicationError
            };
            await _context.ErrorLogs.AddAsync(log).ConfigureAwait(false);

            return await _context.SaveChangesAsync().ConfigureAwait(false) == 1
                ? log
                : null;
        }

    }

}
