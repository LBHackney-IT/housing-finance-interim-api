using Amazon.DynamoDBv2.Model;
using Hackney.Shared.Tenure.Domain;
using System.Collections.Generic;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class TenureInformationPagination
    {
        public Dictionary<string, AttributeValue> LastKey { get; set; }
        public List<TenureInformation> TenuresInformation { get; set; }
    }
}
