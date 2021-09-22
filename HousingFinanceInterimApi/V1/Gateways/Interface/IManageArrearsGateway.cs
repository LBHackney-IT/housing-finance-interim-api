using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IManageArrearsGateway
    {
        public Task RefreshManageArrearsTenancyAgreement();
    }
}
