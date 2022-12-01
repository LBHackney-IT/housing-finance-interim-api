using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The UP Cash load gateway implementation.
    /// </summary>
    /// <seealso cref="IUPHousingCashLoadGateway" />
    public class UPHousingCashLoadGateway : IUPHousingCashLoadGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPHousingCashLoadGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UPHousingCashLoadGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> LoadHousingFiles()
        {
            await _context.LoadHousingFiles().ConfigureAwait(false);
            return true;
        }

        public async Task<List<string>> GetAcademyRefByRentAccount(string rentAccount)
        {
            var housingCashLoads = await _context.UPHousingCashLoads
                .Where(x => x.RentAccount == rentAccount)
                .Select(x => x.AcademyClaimRef)
                .Distinct()
                .ToListAsync()
                .ConfigureAwait(false);

            return housingCashLoads;
        }

    }

}
