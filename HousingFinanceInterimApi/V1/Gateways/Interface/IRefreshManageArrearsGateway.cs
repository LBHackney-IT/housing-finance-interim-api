using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The refresh manage arrears gateway interface.
    /// </summary>
    public interface IRefreshManageArrearsGateway
    {

        /// <summary>
        /// Refresh manage arrears items.
        /// </summary>
        public Task RefreshManageArrearsItems();

    }

}
