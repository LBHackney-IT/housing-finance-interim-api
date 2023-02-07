using HousingFinanceInterimApi.V1.Gateways.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using HousingFinanceInterimApi.V1.Domain;
using Hackney.Shared.HousingSearch.Domain.Asset;
using System;
using System.Linq;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Infrastructure.Postgres;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class LoadAssetFromDynamoDbUseCase : ILoadAssetFromDynamoDbUseCase
    {
        private readonly IAssetGateway _assetGateway;

        public LoadAssetFromDynamoDbUseCase(IAssetGateway assetGateway)
        {
            _assetGateway = assetGateway;
        }

        public async Task<AssetPagination> ExecuteAsync()
        {
            var assets = new List<Asset>();
            Dictionary<string, AttributeValue> lastEvaluatedKey = null;

            do
            {
                var assetPagination = await _assetGateway.GetAll(lastEvaluatedKey).ConfigureAwait(false);
                if (assetPagination.Assets.Count > 0)
                {
                    assets.AddRange(assetPagination.Assets);
                }

                lastEvaluatedKey = assetPagination.LastKey;

            } while (lastEvaluatedKey.ContainsKey("id") && lastEvaluatedKey["id"].S != Guid.Empty.ToString());

            var assetsAux = new List<AssetAuxDbEntity>();
            assets.ForEach(a => assetsAux.Add(new AssetAuxDbEntity() { Id = new Guid(a.Id), AssetId = a.AssetId, AssetType = a.AssetType }));

            LoggingHandler.LogInfo($"Clear aux table");
            await _assetGateway.ClearChargesAuxiliary().ConfigureAwait(false);

            LoggingHandler.LogInfo($"Starting bulk insert");
            await _assetGateway.CreateBulkAsync(assetsAux).ConfigureAwait(false);

            LoggingHandler.LogInfo($"Starting merge charges");
            await _assetGateway.MergeAssetsAuxiliary().ConfigureAwait(false);

            return null;
        }
    }
}
