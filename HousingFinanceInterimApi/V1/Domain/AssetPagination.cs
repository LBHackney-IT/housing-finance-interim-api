using Amazon.DynamoDBv2.Model;
using Hackney.Shared.HousingSearch.Domain.Asset;
using System.Collections.Generic;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class AssetPagination
    {
        public Dictionary<string, AttributeValue> LastKey { get; set; }
        public List<Asset> Assets { get; set; }
    }
}
