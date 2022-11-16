using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using EFCore.BulkExtensions;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.Infrastructure.Postgres;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class TenureInformationGateway : ITenureInformationGateway
    {
        private readonly IAmazonDynamoDB _amazonDynamoDB;
        private readonly HousingFinanceContext _housingFinanceContext;


        public TenureInformationGateway(IAmazonDynamoDB amazonDynamoDB, HousingFinanceContext housingFinanceContext)
        {
            _amazonDynamoDB = amazonDynamoDB;
            _housingFinanceContext = housingFinanceContext;
        }

        public async Task<TenureInformationPagination> GetAll(Dictionary<string, AttributeValue> lastEvaluatedKey = null)
        {
            LoggingHandler.LogInfo($"{nameof(HousingFinanceInterimApi)}.{nameof(TenureInformationGateway)}" +
                                   $".{nameof(GetAll)} Scan started.");

            ScanRequest request = new ScanRequest()
            {
                TableName = "TenureInformation",
                Limit = 10
            };

            if (lastEvaluatedKey != null)
            {
                if (lastEvaluatedKey.ContainsKey("id") && lastEvaluatedKey["id"].S != Guid.Empty.ToString())
                {
                    request.ExclusiveStartKey = lastEvaluatedKey;
                }
            };

            ScanResponse response = await _amazonDynamoDB.ScanAsync(request).ConfigureAwait(false);
            if (response?.Items == null || response.Items.Count == 0)
                throw new Exception($"_dynamoDb.ScanAsync result is null");

            return new TenureInformationPagination()
            {
                LastKey = response?.LastEvaluatedKey,
                TenuresInformation = response?.ToTenureInformation()?.ToList()
            };
        }

        public async Task CreateBulkAsync(IList<TenureInformationAuxDbEntity> tenureInformationAuxDbEntities)
        {
            try
            {
                await _housingFinanceContext.AddRangeAsync(tenureInformationAuxDbEntities).ConfigureAwait(false);
                await _housingFinanceContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task ClearChargesAuxiliary()
        {
            try
            {
                await _housingFinanceContext.TruncateTenuresInformationAuxiliary().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task MergeTenureInformationsAuxiliary()
        {
            try
            {
                await _housingFinanceContext.MergeTenuresInformationAuxiliary().ConfigureAwait(false);
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
