using System.Collections.Generic;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IExampleGateway
    {
        Entity GetEntityById(int id);

        List<Entity> GetAll();
    }
}
