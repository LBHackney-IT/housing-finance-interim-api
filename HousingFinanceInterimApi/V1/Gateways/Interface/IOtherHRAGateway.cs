using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The current rent position gateway interface.
    /// </summary>
    public interface IOtherHRAGateway
    {

        /// <summary>
        /// Saves the garage items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The save result.</returns>
        public Task<int> SaveOtherHRAItems(IList<OtherHRA> items);

    }

}
