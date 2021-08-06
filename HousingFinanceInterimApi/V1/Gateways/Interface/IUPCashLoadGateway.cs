using HousingFinanceInterimApi.V1.Infrastructure;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The UP cash file name gateway interface.
    /// </summary>
    public interface IUPCashLoadGateway
    {
        public Task<bool> LoadCashFiles();
    }

}
