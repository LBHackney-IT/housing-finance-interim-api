using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class RentPositionGateway : IRentPositionGateway
    {

        private readonly DatabaseContext _context;

        public RentPositionGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<string[]>> GetRentPosition()
        {
            try
            {
                return await _context.GetRentPosition().ConfigureAwait(false);
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
