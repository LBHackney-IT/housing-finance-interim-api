using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using Google.Apis.Drive.v3.Data;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class GetBatchLogErrorUseCase : IGetBatchLogErrorUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;

        public GetBatchLogErrorUseCase(IBatchLogGateway batchLogGateway, IBatchLogErrorGateway batchLogErrorGateway)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
        }

        public async Task<IList<BatchLogResponse>> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Getting batch log error");

            var batchLogs = await _batchLogGateway.ListLastMonthAsync().ConfigureAwait(false);
            var batchLogErrors = await _batchLogErrorGateway.ListLastMonthAsync().ConfigureAwait(false);

            foreach (var batchLog in batchLogs)
            {
                batchLog.BatchLogErrors = batchLogErrors.Where(e => e.BatchLogId == batchLog.Id).ToList();
            }

            batchLogs = batchLogs.Where(item => item.BatchLogErrors.Count > 0).ToList();

            LoggingHandler.LogInfo($"{batchLogs}");

            return batchLogs.ToResponse();
        }
    }
}
