using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;

namespace HousingFinanceInterimApi.V1.UseCase
{

    /// <summary>
    /// The log error use case implementation.
    /// </summary>
    public class LogErrorUseCase : ILogErrorUseCase
    {

        /// <summary>
        /// The gateway
        /// </summary>
        private readonly IErrorLogGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogErrorUseCase"/> class.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        public LogErrorUseCase(IErrorLogGateway gateway)
        {
            _gateway = gateway;
        }

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="rowId">The row identifier.</param>
        /// <param name="userFriendlyError">The user friendly error.</param>
        /// <param name="applicationError">The application error.</param>
        /// <returns>
        /// The error log domain object.
        /// </returns>
        public async Task<ErrorLogDomain> ExecuteAsync(string tableName, string rowId, string userFriendlyError,
            string applicationError) => ErrorLogFactory.ToDomain(await _gateway
            .LogAsync(tableName, rowId, userFriendlyError, applicationError)
            .ConfigureAwait(false));

    }

}
