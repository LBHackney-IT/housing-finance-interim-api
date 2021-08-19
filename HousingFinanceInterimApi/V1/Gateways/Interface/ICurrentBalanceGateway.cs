using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface ICurrentBalanceGateway
    {
        public Task UpdateCurrentBalance();
    }
}
