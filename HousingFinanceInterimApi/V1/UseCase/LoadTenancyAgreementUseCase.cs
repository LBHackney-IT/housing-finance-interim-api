using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;

using HousingFinanceInterimApi.Tests.V1.UseCase.Exceptions;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class LoadTenancyAgreementUseCase : ILoadTenancyAgreementUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly ITenancyAgreementGateway _tenancyAgreementGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _tenancyAgreementLabel = "TenancyAgreement";

        public LoadTenancyAgreementUseCase(
            IBatchLogGateway batchLogGateway,
            IBatchLogErrorGateway batchLogErrorGateway,
            ITenancyAgreementGateway tenancyAgreementGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService)
        {
            _batchLogGateway = batchLogGateway;
            _batchLogErrorGateway = batchLogErrorGateway;
            _tenancyAgreementGateway = tenancyAgreementGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting tenancy agreement import");

            const string sheetRange = "A:M";

            var batch = await _batchLogGateway.CreateAsync(_tenancyAgreementLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(_tenancyAgreementLabel).ConfigureAwait(false);

            if (googleFileSettings == null)
                throw new GoogleFileSettingNotFoundException(_tenancyAgreementLabel);

            var sheetProcessingExceptions = new List<Exception>();
            foreach (var sheetName in Enum.GetValues(typeof(RentGroup)))
            {
                var tenancyAgreementAux = await _googleClientService
                    .ReadSheetToEntitiesAsync<TenancyAgreementAuxDomain>(googleFileSettings.GoogleIdentifier, sheetName.ToString(), sheetRange)
                    .ConfigureAwait(false);

                if (tenancyAgreementAux == null || !tenancyAgreementAux.Any())
                {
                    var message = "No tenancy agreement data to import. Sheet name: {sheetName}";
                    var exc = new Exception(message);
                    sheetProcessingExceptions.Add(exc);
                    LoggingHandler.LogError(message);
                    continue;
                }

                await HandleSpreadSheet(batch.Id, tenancyAgreementAux, sheetName.ToString()).ConfigureAwait(false);
            }

            if (sheetProcessingExceptions.Any())
                throw new AggregateException(sheetProcessingExceptions);

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"End tenancy agreement import");
            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }

        private async Task<GoogleFileSettingDomain> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting Google file setting for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} Google file settings found");

            return googleFileSettings.FirstOrDefault();
        }

        private async Task HandleSpreadSheet(long batchId, IList<TenancyAgreementAuxDomain> tenancyAgreementAux, string rentGroup)
        {
            try
            {
                LoggingHandler.LogInfo($"Clear aux table");
                await _tenancyAgreementGateway.ClearTenancyAgreementAuxiliary().ConfigureAwait(false);

                LoggingHandler.LogInfo($"Starting bulk insert");
                await _tenancyAgreementGateway.CreateBulkAsync(tenancyAgreementAux, rentGroup).ConfigureAwait(false);

                LoggingHandler.LogInfo($"Starting merge tenancy agreements");
                await _tenancyAgreementGateway.RefreshTenancyAgreement(batchId).ConfigureAwait(false);

                LoggingHandler.LogInfo("File success");
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleSpreadSheet)}";

                await _batchLogErrorGateway.CreateAsync(batchId, _tenancyAgreementLabel, $"Application error. Not possible to load tenancy agreement").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
