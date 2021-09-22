using System;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The operating balance gateway.
    /// </summary>
    public interface IOperatingBalanceGateway
    {

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
        public Task<IList<OperatingBalance>> ListAsync(DateTime? startDate, DateTime? endDate, int startWeek, int startYear, int endWeek, int endYear);

        public Task GenerateOperatingBalance();

    }

}
