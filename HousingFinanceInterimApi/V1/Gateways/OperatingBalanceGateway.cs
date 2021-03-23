using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class OperatingBalanceGateway : IOperatingBalanceGateway
    {

        private readonly DatabaseContext _context;

        public OperatingBalanceGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lists the operating balances asynchronous.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>
        /// The list of operating balances.
        /// </returns>
        public async Task<IList<OperatingBalance>> ListAsync(DateTime startDate, DateTime endDate)
            => await _context.GetOperatingBalancesAsync(startDate, endDate).ConfigureAwait(false);

    }

}
