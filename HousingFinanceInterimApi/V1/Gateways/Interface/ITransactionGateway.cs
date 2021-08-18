using System;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The transaction gateway.
    /// </summary>
    public interface ITransactionGateway
    {

        /// <summary>
        /// Lists the operating balances asynchronous.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>
        /// The list of operating balances.
        /// </returns>
        public Task<IList<Transaction>> ListAsync(DateTime? startDate, DateTime? endDate);

        public Task LoadCashFilesTransactions();

        public Task LoadHousingFilesTransactions();

    }

}
