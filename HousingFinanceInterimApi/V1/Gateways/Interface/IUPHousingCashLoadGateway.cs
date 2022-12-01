using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The UP cash file name gateway interface.
    /// </summary>
    public interface IUPHousingCashLoadGateway
    {
        public Task<bool> LoadHousingFiles();
        public Task<List<string>> GetAcademyRefByRentAccount(string rentAccount);
    }

}
