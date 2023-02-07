using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using Hackney.Shared.HousingSearch.Domain.Asset;
using Hackney.Shared.Tenure.Domain;
using TenuredAsset = Hackney.Shared.Tenure.Domain.TenuredAsset;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    public static class ScanResponseExtension
    {
        public static IEnumerable<Asset> ToAssets(this ScanResponse response)
        {
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                yield return new Asset
                {
                    Id = item["id"].S,
                    AssetId = item.ContainsKey("assetId") ? (item["assetId"].NULL ? null : item["assetId"].S) : null,
                    AssetType = item.ContainsKey("assetType") ? (item["assetType"].NULL ? null : item["assetType"].S) : null,
                    Tenure = item.ContainsKey("tenure") ? new Tenure()
                    {
                        Id = item["tenure"].M.ContainsKey("id") ? (item["tenure"].M["id"].NULL ? null : item["tenure"].M["id"].S) : null,
                        PaymentReference = item["tenure"].M.ContainsKey("paymentReference") ? (item["tenure"].M["paymentReference"].NULL ? null : item["tenure"].M["paymentReference"].S) : null,
                    } : null
                };
            }
        }

        public static IEnumerable<TenureInformation> ToTenureInformation(this ScanResponse response)
        {
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                yield return new TenureInformation
                {
                    Id = Guid.Parse(item["id"].S),

                    TenuredAsset = item.ContainsKey("tenuredAsset") ? new TenuredAsset()
                    {
                        FullAddress = item["tenuredAsset"].M.ContainsKey("fullAddress") ? (item["tenuredAsset"].M["fullAddress"].NULL ? null : item["tenuredAsset"].M["fullAddress"].S) : null
                    } : null,

                    TenureType = item.ContainsKey("tenureType") ? new TenureType()
                    {
                        Code = item["tenureType"].M.ContainsKey("code") ? (item["tenureType"].M["code"].NULL ? null : item["tenureType"].M["code"].S) : null,
                        Description = item["tenureType"].M.ContainsKey("description") ? (item["tenureType"].M["description"].NULL ? null : item["tenureType"].M["description"].S) : null
                    } : null,

                    Terminated = item.ContainsKey("terminated") ? new Terminated()
                    {
                        IsTerminated = item["terminated"].M.ContainsKey("isTerminated") && (item["terminated"].M["isTerminated"].NULL ? false : item["terminated"].M["isTerminated"].BOOL),
                        ReasonForTermination = item["terminated"].M.ContainsKey("reasonForTermination") ? (item["terminated"].M["reasonForTermination"].NULL ? null : item["terminated"].M["reasonForTermination"].S.Trim()) : null
                    } : null,

                    PaymentReference = item.ContainsKey("paymentReference") ? (item["paymentReference"].NULL ? null : item["paymentReference"].S) : null,
                    HouseholdMembers = item.ContainsKey("householdMembers") ?
                        item["householdMembers"].NULL ? null :
                        item["householdMembers"].L.Select(m =>
                           new HouseholdMembers
                           {
                               Id = m.M["id"].NULL ? Guid.Empty : Guid.Parse(m.M["id"].S),
                               FullName = m.M.ContainsKey("fullName") ? (m.M["fullName"].NULL ? null : m.M["fullName"].S) : null,
                               IsResponsible = m.M.ContainsKey("isResponsible") && (!m.M["isResponsible"].NULL && m.M["isResponsible"].BOOL)
                           }) : null
                };
            }
        }
    }
}
