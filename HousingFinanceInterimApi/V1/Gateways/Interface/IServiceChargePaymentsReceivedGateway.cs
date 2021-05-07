using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The current rent position gateway interface.
    /// </summary>
    public interface IServiceChargePaymentsReceivedGateway
    {

        /// <summary>
        /// Saves the current rent position items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The save result.</returns>
        public Task<int> SaveServiceChargePaymentsReceivedItems(IList<ServiceChargePaymentsReceived> items);

    }

}
