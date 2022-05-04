using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.Factories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class GoogleFileSettingGateway : IGoogleFileSettingGateway
    {

        private readonly DatabaseContext _context;

        public GoogleFileSettingGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IList<GoogleFileSetting>> ListAsync()
            => await _context.GoogleFileSettings.ToListAsync().ConfigureAwait(false);

        public Task<List<GoogleFileSettingDomain>> GetSettingsByLabel(string label)
        {
            try
            {
                var googleFileSettings = _context.GoogleFileSettings
                    .Where(item => item.Label.Equals(label)).ToList();
                return Task.FromResult(googleFileSettings.ToDomain());
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
