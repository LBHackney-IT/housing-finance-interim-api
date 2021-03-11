using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The Google file setting gateway implementation.
    /// </summary>
    /// <seealso cref="IGoogleFileSettingGateway" />
    public class GoogleFileSettingGateway : IGoogleFileSettingGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleFileSettingGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public GoogleFileSettingGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lists the google file settings asynchronous.
        /// </summary>
        /// <returns>
        /// The list of Google file settings.
        /// </returns>
        public async Task<IList<GoogleFileSetting>> ListAsync()
            => await _context.GoogleFileSettings.ToListAsync().ConfigureAwait(false);

    }

}
