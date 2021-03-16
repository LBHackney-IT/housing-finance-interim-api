using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The rent breakdown gateway interface.
    /// </summary>
    public interface IRentBreakdownGateway
    {

        /// <summary>
        /// Saves the rent breakdown items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The save result.</returns>
        public Task<int> SaveRentBreakdownItems(IList<RentBreakdown> items);

    }

}
