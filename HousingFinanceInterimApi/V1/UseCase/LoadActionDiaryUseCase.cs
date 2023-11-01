using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Exceptions;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class LoadActionDiaryUseCase : ILoadActionDiaryUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly IActionDiaryGateway _actionDiaryGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

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
            LoggingHandler.LogInfo($"Starting action diary import");

            const string sheetName = "Active";
            const string sheetRange = "A:H";

            var batch = await _batchLogGateway.CreateAsync(_actionDiaryLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(_actionDiaryLabel).ConfigureAwait(false);

            if (googleFileSettings == null)
                throw new GoogleFileSettingNotFoundException(_actionDiaryLabel);

            var actionDiaryAux = await _googleClientService
                .ReadSheetToEntitiesAsync<ActionDiaryAuxDomain>(googleFileSettings.GoogleIdentifier, sheetName, sheetRange)
                .ConfigureAwait(false);

            if (!actionDiaryAux.Any())
            {
                LoggingHandler.LogWarning(
                        $"No action diary data to import. Sheet name: ({sheetName})");
                LoggingHandler.LogInfo($"END sheet {sheetName}");
                return new StepResponse() { Continue = false, NextStepTime = DateTime.UtcNow.AddSeconds(int.Parse(_waitDuration)) };
            }

            await HandleSpreadSheet(batch.Id, actionDiaryAux).ConfigureAwait(false);

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"End action diary import");
            return new StepResponse() { Continue = true, NextStepTime = DateTime.UtcNow.AddSeconds(int.Parse(_waitDuration)) };
        }

        private async Task<GoogleFileSettingDomain> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting Google file setting for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} Google file settings found");

            return googleFileSettings.FirstOrDefault();
        }

        private async Task HandleSpreadSheet(long batchId, IList<ActionDiaryAuxDomain> actionDiaryAux)
        {
            try
            {
                LoggingHandler.LogInfo($"Clear aux table");
                await _actionDiaryGateway.ClearActionDiaryAuxiliary().ConfigureAwait(false);

                LoggingHandler.LogInfo($"Starting bulk insert");
                await _actionDiaryGateway.CreateBulkAsync(actionDiaryAux).ConfigureAwait(false);

                LoggingHandler.LogInfo($"Starting merge action diary");
                await _actionDiaryGateway.LoadActionDiary().ConfigureAwait(false);

                LoggingHandler.LogInfo("File success");
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleSpreadSheet)}";

                await _batchLogErrorGateway.CreateAsync(batchId, _actionDiaryLabel, $"Application error. Not possible to load action diary").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
