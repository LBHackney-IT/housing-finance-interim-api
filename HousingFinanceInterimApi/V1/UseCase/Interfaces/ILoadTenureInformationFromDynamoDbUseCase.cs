using HousingFinanceInterimApi.V1.Gateways.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{
    public interface ILoadTenureInformationFromDynamoDbUseCase
    {
        Task<TenureInformationPagination> ExecuteAsync();
    }
}
