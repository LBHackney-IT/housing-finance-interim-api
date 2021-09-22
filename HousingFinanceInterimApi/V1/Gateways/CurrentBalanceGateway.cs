using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class CurrentBalanceGateway : ICurrentBalanceGateway
    {
        private readonly DatabaseContext _context;

        public CurrentBalanceGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task UpdateCurrentBalance()
        {
            try
            {
                await _context.UpdateCurrentBalance().ConfigureAwait(false);
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
