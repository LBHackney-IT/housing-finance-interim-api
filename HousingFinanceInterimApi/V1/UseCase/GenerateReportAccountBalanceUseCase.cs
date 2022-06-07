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
    public class GenerateReportAccountBalanceUseCase : IGenereteReportAccountBalanceUseCase
    {
        private readonly IBatchReportAccountBalanceGateway _batchReportAccountBalanceGateway;
        private readonly IReportAccountBalanceGateway _reportAccountBalanceGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private readonly string _rentPositionLabel = "ReportAccountBalance";

        public GenerateReportAccountBalanceUseCase(IBatchReportAccountBalanceGateway batchReportAccountBalanceGateway,
            IReportAccountBalanceGateway reportAccountBalanceGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService)
        {
            _batchReportAccountBalanceGateway = batchReportAccountBalanceGateway;
            _reportAccountBalanceGateway = reportAccountBalanceGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Checking if exist report in the queue");
            var accountBalancesItems = await _batchReportAccountBalanceGateway.ListPendingAsync().ConfigureAwait(false);

            if (!accountBalancesItems.Any())
            {
                return new StepResponse() { Continue = false, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
            }

            var accountBalancesItem = accountBalancesItems.OrderBy(x => x.StartTime).First();

            var rentgroup = string.IsNullOrEmpty(accountBalancesItem.RentGroup) ? "ALL" : accountBalancesItem.RentGroup.Trim();
            var reportDate = accountBalancesItem.ReportDate.ToString("yyyyMMdd");



            var googleFileSetting = await GetGoogleFileSetting(_rentPositionLabel).ConfigureAwait(false);
            if (googleFileSetting == null)
            {
                LoggingHandler.LogInfo($"Output folder not found");
                return new StepResponse()
                {
                    Continue = false,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }

            var folder = await _googleClientService
                .GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            var fileName = $"Account_Balance_{rentgroup}_{reportDate}_{accountBalancesItem.Id}.csv";

            var reportAccountBalances = await _reportAccountBalanceGateway.ListAsync(accountBalancesItem.ReportDate, accountBalancesItem.RentGroup).ConfigureAwait(false);
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

            var upload = await _googleClientService.UploadCsvFile(convertedReportAccountBalance, fileName, googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            System.Threading.Thread.Sleep(int.Parse(_waitDuration));

            var file = await _googleClientService
                .GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier, fileName)
                .ConfigureAwait(false);

            var fileLink = $"https://drive.google.com/file/d/{file.Id}";
            await _batchReportAccountBalanceGateway.SetToSuccessAsync(accountBalancesItem.Id, fileLink).ConfigureAwait(false);

            return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }

        private async Task<GoogleFileSettingDomain> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"Getting Google file settings for '{label}' label");
            var googleFileSettings = await _googleFileSettingGateway.GetSettingsByLabel(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"{googleFileSettings.Count} google file settings found");

            return googleFileSettings.FirstOrDefault();
        }
    }
}
