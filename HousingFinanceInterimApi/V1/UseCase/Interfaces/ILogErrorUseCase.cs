using HousingFinanceInterimApi.V1.Domain;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The log error use case interface.
    /// </summary>
    public interface ILogErrorUseCase
    {

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="rowId">The row identifier.</param>
        /// <param name="userFriendlyError">The user friendly error.</param>
        /// <param name="applicationError">The application error.</param>
        /// <returns>The error log domain object.</returns>
        public Task<ErrorLogDomain> ExecuteAsync(string tableName, string rowId, string userFriendlyError,
            string applicationError);

    }

}
