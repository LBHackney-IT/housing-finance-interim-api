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
    public class LoadTenancyAgreementUseCase : ILoadTenancyAgreementUseCase
    {
        private readonly IBatchLogGateway _batchLogGateway;
        private readonly IBatchLogErrorGateway _batchLogErrorGateway;
        private readonly ITenancyAgreementGateway _tenancyAgreementGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));
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
            LoggingHandler.LogInfo($"STARTING TENANCY AGREEMENT IMPORT");

            const string sheetName = "Active";
            const string sheetRange = "A:T";

            var batch = await _batchLogGateway.CreateAsync(_tenancyAgreementLabel).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(_tenancyAgreementLabel).ConfigureAwait(false);

            if (googleFileSettings == null)
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };

            var tenancyAgreementAux = await _googleClientService
                .ReadSheetToEntitiesAsync<TenancyAgreementAuxDomain>(googleFileSettings.GoogleIdentifier, sheetName, sheetRange)
                .ConfigureAwait(false);

            if (!tenancyAgreementAux.Any())
            {
                LoggingHandler.LogInfo($"NO TENANCY AGREEMENT DATA TO IMPORT");
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
            }

            await HandleSpreadSheet(batch.Id, tenancyAgreementAux).ConfigureAwait(false);

            await _batchLogGateway.SetToSuccessAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"END TENANCY AGREEMENT IMPORT");
            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }

        private async Task<GoogleFileSettingDomain> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"GETTING GOOGLE FILE SETTING FOR '{label}' LABEL");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} GOOGLE FILE SETTINGS FOUND");

            return googleFileSettings.FirstOrDefault();
        }

        private async Task HandleSpreadSheet(long batchId, IList<TenancyAgreementAuxDomain> tenancyAgreementAux)
        {
            try
            {
                await _tenancyAgreementGateway.ClearTenancyAgreementAuxiliary().ConfigureAwait(false);

                var skip = 0;
                var failure = false;
                List<TenancyAgreementAuxDomain> batchTenancyAgreement;

                do
                {
                    batchTenancyAgreement = tenancyAgreementAux.Skip(skip).Take(_batchSize).ToList();
                    skip += _batchSize;

                    if (!batchTenancyAgreement.Any()) continue;

                    var bulkResult = await _tenancyAgreementGateway.CreateBulkAsync(batchTenancyAgreement)
                        .ConfigureAwait(false);

                    if (bulkResult == null)
                    {
                        failure = true;
                        const string message = "FAILURE TO LOAD ALL ROWS";
                        LoggingHandler.LogError(message);
                        await _batchLogErrorGateway.CreateAsync(batchId, _tenancyAgreementLabel, message).ConfigureAwait(false);
                        continue;
                    }
                    LoggingHandler.LogInfo($"FILE LINES CREATED {bulkResult.Count}");
                }
                while (batchTenancyAgreement.Any() && !failure);

                if (!failure)
                {
                    await _tenancyAgreementGateway.RefreshTenancyAgreement().ConfigureAwait(false);
                    LoggingHandler.LogInfo("FILE SUCCESS");
                }
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleSpreadSheet)}";

                await _batchLogErrorGateway.CreateAsync(batchId, _tenancyAgreementLabel, $"APPLICATION ERROR. NOT POSSIBLE TO LOAD TENANCY AGREEMENT").ConfigureAwait(false);

                LoggingHandler.LogError($"{namespaceLabel} APPLICATION ERROR");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
