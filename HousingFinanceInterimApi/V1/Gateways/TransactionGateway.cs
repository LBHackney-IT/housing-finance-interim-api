using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class TransactionGateway : ITransactionGateway
    {

        private readonly DatabaseContext _context;

        public TransactionGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lists the operating balances asynchronous.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="startWeek">The start week.</param>
        /// <param name="startYear">The start year.</param>
        /// <param name="endWeek">The end week.</param>
        /// <param name="endYear">The end year.</param>
        /// <returns>
        /// The list of operating balances.
        /// </returns>
        public async Task<IList<Transaction>> ListAsync(DateTime? startDate, DateTime? endDate)
        {
            var results = await _context.GetTransactionsAsync(startDate, endDate).ConfigureAwait(false);

            return results;
        }

        public async Task<bool> LoadCashFilesTransactions()
        {
            await _context.LoadCashFileTransactions().ConfigureAwait(false);
            return true;
        }

    }

}
