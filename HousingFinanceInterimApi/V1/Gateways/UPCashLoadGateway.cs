using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The UP Cash load gateway implementation.
    /// </summary>
    /// <seealso cref="IUPCashLoadGateway" />
    public class UPCashLoadGateway : IUPCashLoadGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCashLoadGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UPCashLoadGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> LoadCashFiles()
        {
            await _context.LoadCashFiles().ConfigureAwait(false);
            return true;
        }

    }

}
