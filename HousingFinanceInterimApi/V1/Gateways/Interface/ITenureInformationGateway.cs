using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.DynamoDBv2.Model;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure.Postgres;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface ITenureInformationGateway
    {
        public Task<TenureInformationPagination> GetAll(Dictionary<string, AttributeValue> lastEvaluatedKey = null);
        public Task CreateBulkAsync(IList<TenureInformationAuxDbEntity> tenureInformationAuxDbEntities);
        public Task ClearChargesAuxiliary();
        public Task MergeTenureInformationsAuxiliary();
    }
}
