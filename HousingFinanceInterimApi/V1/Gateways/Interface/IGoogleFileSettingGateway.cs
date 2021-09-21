using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IGoogleFileSettingGateway
    {
        public Task<IList<GoogleFileSetting>> ListAsync();

        Task<List<GoogleFileSettingDomain>> GetSettingsByLabel(string label);
    }

}
