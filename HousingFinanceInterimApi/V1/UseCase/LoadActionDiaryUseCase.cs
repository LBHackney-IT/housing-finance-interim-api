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
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class LoadActionDiaryUseCase : ILoadActionDiaryUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IActionDiaryGateway _actionDiaryGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _actionDiaryLabel = "ActionDiary";

        public LoadActionDiaryUseCase(
            IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            IActionDiaryGateway actionDiaryGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _actionDiaryGateway = actionDiaryGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"STARTING ACTION DIARY IMPORT");

            const string sheetName = "Active";
            const string sheetRange = "A:H";

            var batch = await _batchLogGateway.CreateAsync(_actionDiaryLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(_actionDiaryLabel).ConfigureAwait(false);

            if (googleFileSettings == null)
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };

            var actionDiaryAux = await _googleClientService
                .ReadSheetToEntitiesAsync<ActionDiaryAuxDomain>(googleFileSettings.GoogleIdentifier, sheetName, sheetRange)
                .ConfigureAwait(false);

            if (!actionDiaryAux.Any())
            {
                LoggingHandler.LogInfo($"NO ACTION DIARY DATA TO IMPORT");
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
            }

            await HandleSpreadSheet(batch.Id, actionDiaryAux).ConfigureAwait(false);

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"END ACTION DIARY IMPORT");
            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }

        private async Task<GoogleFileSettingDomain> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"GETTING GOOGLE FILE SETTING FOR '{label}' LABEL");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} GOOGLE FILE SETTINGS FOUND");

            return googleFileSettings.FirstOrDefault();
        }

        private async Task HandleSpreadSheet(long batchId, IList<ActionDiaryAuxDomain> actionDiaryAux)
        {
            try
            {
                await _actionDiaryGateway.ClearActionDiaryAuxiliary().ConfigureAwait(false);

                var skip = 0;
                var failure = false;
                List<ActionDiaryAuxDomain> batchActionDiary;

                do
                {
                    batchActionDiary = actionDiaryAux.Skip(skip).Take(_batchSize).ToList();
                    skip += _batchSize;

                    if (!batchActionDiary.Any()) continue;

                    var bulkResult = await _actionDiaryGateway.CreateBulkAsync(batchActionDiary)
                        .ConfigureAwait(false);

                    if (bulkResult == null)
                    {
                        failure = true;
                        const string message = "FAILURE TO LOAD ALL ROWS";
                        LoggingHandler.LogError(message);
                        await _batchLogErrorGateway.CreateAsync(batchId, _actionDiaryLabel, message).ConfigureAwait(false);
                        continue;
                    }
                    LoggingHandler.LogInfo($"FILE LINES CREATED {bulkResult.Count}");
                }
                while (batchActionDiary.Any() && !failure);

                if (!failure)
                {
                    await _actionDiaryGateway.LoadActionDiary().ConfigureAwait(false);
                    LoggingHandler.LogInfo("FILE SUCCESS");
                }
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleSpreadSheet)}";

                await _batchLogErrorGateway.CreateAsync(batchId, _actionDiaryLabel, $"APPLICATION ERROR. NOT POSSIBLE TO LOAD ACTION DIARY").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} APPLICATION ERROR");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
