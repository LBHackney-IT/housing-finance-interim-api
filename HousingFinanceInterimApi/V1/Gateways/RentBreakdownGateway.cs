using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The rent breakdown gateway implementation.
    /// </summary>
    /// <seealso cref="IRentBreakdownGateway" />
    public class RentBreakdownGateway : IRentBreakdownGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentBreakdownGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public RentBreakdownGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Saves the rent breakdown items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>
        /// The save result.
        /// </returns>
        public async Task<int> SaveRentBreakdownItems(IList<RentBreakdown> items)
        {
            // Delete data first
            await _context.DeleteRentBreakdowns().ConfigureAwait(false);

            // Add data
            await _context.RentBreakdowns.AddRangeAsync(items).ConfigureAwait(false);

            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }

    }

}
