using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The current rent position gateway implementation.
    /// </summary>
    /// <seealso cref="IOtherHRAGateway" />
    public class OtherHRAGateway : IOtherHRAGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OtherHRAGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public OtherHRAGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Saves the current rent position items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>
        /// The save result.
        /// </returns>
        public async Task<int> SaveOtherHRAItems(IList<OtherHRA> items)
        {
            // Delete data first
            await _context.DeleteOtherHRA().ConfigureAwait(false);

            await _context.OtherHRA.AddRangeAsync(items).ConfigureAwait(false);

            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }

    }

}
