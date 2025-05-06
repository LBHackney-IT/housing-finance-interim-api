using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class NightlyProcessLogUseCase : INightlyProcessLogUseCase
    {
        private readonly INightlyProcessLogGateway _gateway;

        public NightlyProcessLogUseCase(INightlyProcessLogGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<List<NightlyProcessLogResponse>> ExecuteAsync(DateTime createdDate)
        {
            var logs = await _gateway.GetByDateCreatedAsync(createdDate).ConfigureAwait(false);
            return logs.Select(log => new NightlyProcessLogResponse
            {
                Id = log.Id,
                LogGroupName = log.LogGroupName,
                Timestamp = log.Timestamp,
                IsSuccess = log.IsSuccess,
                DateCreated = log.DateCreated
            }).ToList();
        }
    }
}
