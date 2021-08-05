using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="startWeek">The start week.</param>
        /// <param name="startYear">The start year.</param>
        /// <param name="endWeek">The end week.</param>
        /// <param name="endYear">The end year.</param>
        /// <returns>
        /// The list of operating balances.
        /// </returns>
        public async Task<IList<OperatingBalance>> ListAsync(DateTime? startDate, DateTime? endDate, int startWeek, int startYear, int endWeek, int endYear)
        {
            var results = await _context.GetOperatingBalancesAsync(startDate, endDate, startWeek, startYear, endWeek, endYear).ConfigureAwait(false);

            OperatingBalance totalBalance = new OperatingBalance
            {
                RentGroup = "Totals",
                TotalCharged = results.Sum(item => item.TotalCharged),
                TotalPaid = results.Sum(item => item.TotalPaid),
                TotalBalance = results.Sum(item => item.TotalBalance),
                ChargedYTD = results.Sum(item => item.ChargedYTD),
                PaidYTD = results.Sum(item => item.PaidYTD),
                ArrearsYTD = results.Sum(item => item.ArrearsYTD),
            };
            results.Add(totalBalance);

            return results;
        }

    }

}
