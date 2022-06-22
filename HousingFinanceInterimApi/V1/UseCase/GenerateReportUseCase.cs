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
    public class GenerateReportUseCase : IGenereteReportAccountBalanceUseCase
    {
        private readonly IBatchReportGateway _batchReportGateway;
        private readonly IReportAccountBalanceGateway _reportAccountBalanceGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private readonly int _sleepDuration = 30000;

        private const string ReportAccountBalanceByDateLabel = "ReportAccountBalanceByDate";

        public GenerateReportUseCase(IBatchReportGateway batchReportGateway,
            IReportAccountBalanceGateway reportAccountBalanceGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService)
        {
            _batchReportGateway = batchReportGateway;
            _reportAccountBalanceGateway = reportAccountBalanceGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Checking if exist report in the queue");
            var batchReports = await _batchReportGateway.ListPendingAsync().ConfigureAwait(false);

            if (!batchReports.Any())
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };

            var batchReport = batchReports.OrderBy(x => x.StartTime).First();

            switch (batchReport.ReportName)
            {
                case ReportAccountBalanceByDateLabel:
                    await CreateBalanceReportByDate(batchReport).ConfigureAwait(false);
                    break;
                default:
                    LoggingHandler.LogInfo($"Output folder not found");
                    break;
            }

            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }

        private async Task<GoogleFileSettingDomain> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting Google file settings for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} google file settings found");

            return googleFileSettings.FirstOrDefault();
        }

        private async Task<bool> CreateBalanceReportByDate(BatchReportDomain batchReport)
        {
            var rentgroup = string.IsNullOrEmpty(batchReport.RentGroup) ? "ALL" : batchReport.RentGroup.Trim();
            var reportDate = batchReport.ReportDate.Value.ToString("yyyyMMdd");

            var googleFileSetting = await GetGoogleFileSetting(ReportAccountBalanceByDateLabel).ConfigureAwait(false);
            if (googleFileSetting == null)
            {
                LoggingHandler.LogInfo($"Output folder not found");
                return false;
            }

            var folder = await _googleClientService
                .GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            var fileName = $"Account_Balance_{rentgroup}_{reportDate}_{batchReport.Id}.csv";

            var reportAccountBalances = await _reportAccountBalanceGateway.ListAsync(batchReport.ReportDate.Value, batchReport.RentGroup).ConfigureAwait(false);
            List<string[]> convertedReportAccountBalance = reportAccountBalances
                .Select(x => new string[]
                {
                    x.TenancyAgreementRef,
                    x.RentAccount,
                    x.RentGroup,
                    x.TenancyEndDate?.ToString("dd/MM/yyyy"),
                    x.Balance.ToString()
                })
                .ToList();

            convertedReportAccountBalance.Insert(0, new string[]{
                "TenancyAgreementRef",
                "RentAccount",
                "RentGroup",
                "TenancyEndDate",
                "Balance"
            });

            await _googleClientService
                .UploadCsvFile(convertedReportAccountBalance, fileName, googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            System.Threading.Thread.Sleep(_sleepDuration);

            var file = await _googleClientService
                .GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier, fileName)
                .ConfigureAwait(false);

            var fileLink = $"https://drive.google.com/file/d/{file.Id}";
            await _batchReportGateway.SetToSuccessAsync(batchReport.Id, fileLink).ConfigureAwait(false);

            return true;
        }
    }
}
