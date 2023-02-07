using HousingFinanceInterimApi.V1.Gateways.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using HousingFinanceInterimApi.V1.Domain;
using Hackney.Shared.Tenure.Domain;
using System;
using System.Linq;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Infrastructure.Postgres;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class LoadTenureInformationFromDynamoDbUseCase : ILoadTenureInformationFromDynamoDbUseCase
    {
        private readonly ITenureInformationGateway _tenureInformationGateway;

        public LoadTenureInformationFromDynamoDbUseCase(ITenureInformationGateway tenureInformationGateway)
        {
            _tenureInformationGateway = tenureInformationGateway;
        }

        public async Task<TenureInformationPagination> ExecuteAsync()
        {
            var tenureInformations = new List<TenureInformation>();
            Dictionary<string, AttributeValue> lastEvaluatedKey = null;

            do
            {
                var tenureInformationPagination = await _tenureInformationGateway.GetAll(lastEvaluatedKey).ConfigureAwait(false);
                if (tenureInformationPagination.TenuresInformation.Count > 0)
                {
                    tenureInformations.AddRange(tenureInformationPagination.TenuresInformation);
                }

                lastEvaluatedKey = tenureInformationPagination.LastKey;

            } while (lastEvaluatedKey.ContainsKey("id") && lastEvaluatedKey["id"].S != Guid.Empty.ToString());

            var tenureInformationsAux = new List<TenureInformationAuxDbEntity>();
            tenureInformations.ForEach(a => tenureInformationsAux.Add(new TenureInformationAuxDbEntity() { Id = a.Id, PaymentReference = a.PaymentReference }));

            LoggingHandler.LogInfo($"Clear aux table");
            await _tenureInformationGateway.ClearChargesAuxiliary().ConfigureAwait(false);

            LoggingHandler.LogInfo($"Starting bulk insert");
            await _tenureInformationGateway.CreateBulkAsync(tenureInformationsAux).ConfigureAwait(false);

            LoggingHandler.LogInfo($"Starting merge charges");
            await _tenureInformationGateway.MergeTenureInformationsAuxiliary().ConfigureAwait(false);

            return null;
        }
    }
}
