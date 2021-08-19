using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The refresh manage arrears gateway implementation.
    /// </summary>
    /// <seealso cref="IManageArrearsGateway" />
    public class ManageArrearsGateway : IManageArrearsGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageArrearsGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ManageArrearsGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task RefreshManageArrearsTenancyAgreement()
        {
            try
            {
                await _context.RefreshManageArrearsTenancyAgreement().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }
    }
}
