using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The refresh manage arrears gateway implementation.
    /// </summary>
    /// <seealso cref="IRefreshManageArrearsGateway" />
    public class RefreshManageArrearsGateway : IRefreshManageArrearsGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshManageArrearsGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public RefreshManageArrearsGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Refresh manage arrears items.
        /// </summary>
        public async Task RefreshManageArrearsItems()
        {
            // Delete data first
            await _context.GenerateSpreadsheetTransaction().ConfigureAwait(false);

            await _context.RefreshManageArrearsMember().ConfigureAwait(false);

            await _context.RefreshManageArrearsProperty().ConfigureAwait(false);

            await _context.RefreshManageArrearsTenancyAgreement().ConfigureAwait(false);
        }

    }

}
