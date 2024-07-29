using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using GD = Google.Apis.Drive.v3.Data;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Helpers;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class GenerateReportUseCase : IGenerateReportUseCase
    {
        private readonly IBatchReportGateway _batchReportGateway;
        private readonly IReportGateway _reportGateway;
        private readonly ITransactionGateway _transactionGateway;
        //private readonly IReportAccountBalanceGateway _reportAccountBalanceGateway;
        //private readonly IReportChargesGateway _reportChargesGateway;
        //private readonly IReportCashImportGateway _reportCashImportGateway;
        //private readonly IReportSuspenseAccountGateway _reportSuspenseAccountGateway;
        private readonly IGoogleFileSettingGateway _googleFileSettingGateway;
        private readonly IGoogleClientService _googleClientService;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private readonly int _sleepDuration; // ms
        private readonly int _retryInterval; // ms
        private const string ReportAccountBalanceByDateLabel = "ReportAccountBalanceByDate";
        private const string ReportChargesLabel = "ReportCharges";
        private const string ReportOperatingBalancesByRentAccount = "ReportOperatingBalancesByRentAccount";
        private const string ReportItemisedTransactionsLabel = "ReportItemisedTransactions";
        private const string ReportCashSuspenseLabel = "ReportCashSuspense";
        private const string ReportCashImportLabel = "ReportCashImport";
        private const string ReportHousingBenefitAcademyLabel = "ReportHousingBenefitAcademy";

        public GenerateReportUseCase(
            IBatchReportGateway batchReportGateway,
            IReportGateway reportGateway,
            ITransactionGateway transactionGateway,
            IGoogleFileSettingGateway googleFileSettingGateway,
            IGoogleClientService googleClientService,
            int sleepDuration = 30_000,
            int retryInterval = 200
            )
        {
            _batchReportGateway = batchReportGateway;
            _reportGateway = reportGateway;
            _transactionGateway = transactionGateway;
            _googleFileSettingGateway = googleFileSettingGateway;
            _googleClientService = googleClientService;
            _sleepDuration = sleepDuration;
            _retryInterval = retryInterval;
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
                case ReportChargesLabel:
                    await CreateChargesReport(batchReport).ConfigureAwait(false);
                    break;
                case ReportOperatingBalancesByRentAccount:
                    await CreateOperatingBalancesByRentAccount(batchReport).ConfigureAwait(false);
                    break;
                case ReportItemisedTransactionsLabel:
                    await CreateItemisedTransactionsReport(batchReport).ConfigureAwait(false);
                    break;
                case ReportCashSuspenseLabel:
                    await CreateCashSuspenseReport(batchReport).ConfigureAwait(false);
                    break;
                case ReportCashImportLabel:
                    await CreateCashImportReport(batchReport).ConfigureAwait(false);
                    break;
                case ReportHousingBenefitAcademyLabel:
                    await CreateHousingBenefitAcademyReport(batchReport).ConfigureAwait(false);
                    break;
                default:
                    LoggingHandler.LogInfo($"Report label not found");
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

        private async Task CreateBalanceReportByDate(BatchReportDomain batchReport)
        {
            var rentgroup = string.IsNullOrEmpty(batchReport.RentGroup) ? "ALL" : batchReport.RentGroup.Trim();
            var reportDate = batchReport.ReportDate.Value.ToString("yyyyMMdd");

            var googleFileSetting = await GetGoogleFileSetting(ReportAccountBalanceByDateLabel).ConfigureAwait(false);
            if (googleFileSetting == null)
            {
                LoggingHandler.LogInfo($"Output folder not found");
                await _batchReportGateway.SetStatusAsync(batchReport.Id, "Output folder not found", false).ConfigureAwait(false);
                return;
            }

            var folder = await _googleClientService
                .GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            var fileName = $"Account_Balance_{rentgroup}_{reportDate}_{batchReport.Id}.csv";
            var reportAccountBalances = (List<string[]>) await _reportGateway.GetReportAccountBalanceAsync(batchReport.ReportDate.Value, batchReport.RentGroup).ConfigureAwait(false);

            await _googleClientService
                .UploadCsvFile(reportAccountBalances, fileName, googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            System.Threading.Thread.Sleep(_sleepDuration);

            var file = await _googleClientService
                .GetFileByNameInDriveAsync(googleFileSetting.GoogleIdentifier, fileName)
                .ConfigureAwait(false);

            var fileLink = $"https://drive.google.com/file/d/{file.Id}";
            await _batchReportGateway.SetStatusAsync(batchReport.Id, fileLink, true).ConfigureAwait(false);
        }

        private async Task CreateChargesReport(BatchReportDomain batchReport)
        {
            var googleFileSetting = await GetGoogleFileSetting(ReportChargesLabel).ConfigureAwait(false);
            if (googleFileSetting == null)
            {
                LoggingHandler.LogInfo($"Output folder not found");
                await _batchReportGateway.SetStatusAsync(batchReport.Id, "Output folder not found", false).ConfigureAwait(false);
                return;
            }

            var folder = await _googleClientService
                .GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            var fileName = "";
            var reportCharges = new List<string[]>();

            if (!string.IsNullOrEmpty(batchReport.RentGroup))
            {
                fileName = $"Charges_{batchReport.RentGroup}_{batchReport.ReportYear}_{batchReport.Id}.csv";
                reportCharges = (List<string[]>) await _reportGateway.GetChargesByYearAndRentGroupAsync(batchReport.ReportYear.Value, batchReport.RentGroup).ConfigureAwait(false);
            }
            else if (!string.IsNullOrEmpty(batchReport.Group))
            {
                fileName = $"Charges_{batchReport.Group}_{batchReport.ReportYear}_{batchReport.Id}.csv";
                reportCharges = (List<string[]>) await _reportGateway.GetChargesByGroupTypeAsync(batchReport.ReportYear.Value, batchReport.Group).ConfigureAwait(false);
            }
            else
            {
                fileName = $"Charges_{batchReport.ReportYear}_{batchReport.Id}.csv";
                reportCharges = (List<string[]>) await _reportGateway.GetChargesByYearAsync(batchReport.ReportYear.Value).ConfigureAwait(false);
            }

            await _googleClientService
                .UploadCsvFile(reportCharges, fileName, googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            System.Threading.Thread.Sleep(_sleepDuration);

            var file = await _googleClientService
                .GetFileByNameInDriveAsync(googleFileSetting.GoogleIdentifier, fileName)
                .ConfigureAwait(false);

            var fileLink = $"https://drive.google.com/file/d/{file.Id}";
            await _batchReportGateway.SetStatusAsync(batchReport.Id, fileLink, true).ConfigureAwait(false);
        }

        private async Task CreateOperatingBalancesByRentAccount(BatchReportDomain batchReport)
        {
            var opBalsByRentAccFolderGFS = await GetGoogleFileSetting(ReportOperatingBalancesByRentAccount).ConfigureAwait(false);
            if (opBalsByRentAccFolderGFS == null)
            {
                LoggingHandler.LogInfo($"Output folder not found");
                await _batchReportGateway.SetStatusAsync(batchReport.Id, "Output folder not found", false).ConfigureAwait(false);
                return;
            }

            var fileName = $"Operating_Balances_by_Rent_Account_{batchReport.RentGroup}" +
                $"_y{batchReport.ReportYear}_s{batchReport.ReportStartWeekOrMonth}" +
                $"_e{batchReport.ReportEndWeekOrMonth}_id{batchReport.Id}.csv";

            var reportCharges = await _transactionGateway
                .GetPRNTransactions(batchReport.ExtractPRNTransactionArgs())
                .ConfigureAwait(false);

            var csvFile = CSVHelper.ToCSVInMemoryFile(reportCharges, fileName);

            await _googleClientService
                .UploadFileOrThrow(csvFile, opBalsByRentAccFolderGFS.GoogleIdentifier)
                .ConfigureAwait(false);

            GD.File file = null;

            var waitDurationInSeconds = _sleepDuration / 1000;
            var cuttoffTime = DateTime.Now.AddSeconds(waitDurationInSeconds);

            do
            {
                System.Threading.Thread.Sleep(1000);

                file = await _googleClientService
                    .GetFileByNameInDriveAsync(opBalsByRentAccFolderGFS.GoogleIdentifier, fileName)
                    .ConfigureAwait(false);
            }
            while (file is null && DateTime.Now < cuttoffTime);

            if (file is null)
            {
                LoggingHandler.LogInfo($"File with name: '{fileName}' was not found within the {opBalsByRentAccFolderGFS.GoogleIdentifier} directory.");
                await _batchReportGateway.SetStatusAsync(batchReport.Id, "Uploaded report file not found", false).ConfigureAwait(false);
                return;
            }

            var fileLink = $"https://drive.google.com/file/d/{file.Id}";
            await _batchReportGateway.SetStatusAsync(batchReport.Id, fileLink, true).ConfigureAwait(false);
        }

        private async Task CreateItemisedTransactionsReport(BatchReportDomain batchReport)
        {
            var itemisedTransactionFolderGFS = await GetGoogleFileSetting(ReportItemisedTransactionsLabel).ConfigureAwait(false);
            if (itemisedTransactionFolderGFS == null)
            {
                LoggingHandler.LogInfo($"Output folder not found");
                await _batchReportGateway.SetStatusAsync(batchReport.Id, "Output folder not found", false).ConfigureAwait(false);
                return;
            }

            var fileName = $"Itemised_Transactions_{batchReport.TransactionType}_{batchReport.ReportYear}_{batchReport.Id}.csv";
            var reportCharges = (List<string[]>) await _reportGateway
                .GetItemisedTransactionsByYearAndTransactionTypeAsync(batchReport.ReportYear.Value, batchReport.TransactionType)
                .ConfigureAwait(false);

            await _googleClientService
                .UploadCsvFile(reportCharges, fileName, itemisedTransactionFolderGFS.GoogleIdentifier)
                .ConfigureAwait(false);

            GD.File file = null;

            var waitDurationInSeconds = _sleepDuration / 1000;
            var cuttoffTime = DateTime.Now.AddSeconds(waitDurationInSeconds);

            do
            {
                System.Threading.Thread.Sleep(_retryInterval);

                file = await _googleClientService
                    .GetFileByNameInDriveAsync(itemisedTransactionFolderGFS.GoogleIdentifier, fileName)
                    .ConfigureAwait(false);
            }
            while (file is null && DateTime.Now < cuttoffTime);

            if (file is null)
            {
                LoggingHandler.LogInfo($"File with name: '{fileName}' was not found within the {itemisedTransactionFolderGFS.GoogleIdentifier} directory.");
                await _batchReportGateway.SetStatusAsync(batchReport.Id, "Uploaded report file not found", false).ConfigureAwait(false);
                return;
            }

            var fileLink = $"https://drive.google.com/file/d/{file.Id}";
            await _batchReportGateway.SetStatusAsync(batchReport.Id, fileLink, true).ConfigureAwait(false);
        }

        private async Task CreateCashSuspenseReport(BatchReportDomain batchReport)
        {
            var googleFileSetting = await GetGoogleFileSetting(ReportCashSuspenseLabel).ConfigureAwait(false);
            if (googleFileSetting == null)
            {
                LoggingHandler.LogInfo($"Output folder not found");
                await _batchReportGateway.SetStatusAsync(batchReport.Id, "Output folder not found", false).ConfigureAwait(false);
                return;
            }

            var folder = await _googleClientService
                .GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            var reportSuspenseAccount = (List<string[]>) await _reportGateway
               .GetCashSuspenseAccountByYearAsync(batchReport.ReportYear.Value, batchReport.Group).ConfigureAwait(false);

            var fileName = $"Cash_Suspense_{batchReport.Group}_{batchReport.ReportYear.Value}_{batchReport.Id}.csv";

            await _googleClientService
                .UploadCsvFile(reportSuspenseAccount, fileName, googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            System.Threading.Thread.Sleep(_sleepDuration);

            var file = await _googleClientService
                .GetFileByNameInDriveAsync(googleFileSetting.GoogleIdentifier, fileName)
                .ConfigureAwait(false);

            var fileLink = $"https://drive.google.com/file/d/{file.Id}";
            await _batchReportGateway.SetStatusAsync(batchReport.Id, fileLink, true).ConfigureAwait(false);
        }

        private async Task CreateCashImportReport(BatchReportDomain batchReport)
        {
            var googleFileSetting = await GetGoogleFileSetting(ReportCashImportLabel).ConfigureAwait(false);
            if (googleFileSetting == null)
            {
                LoggingHandler.LogInfo($"Output folder not found");
                await _batchReportGateway.SetStatusAsync(batchReport.Id, "Output folder not found", false).ConfigureAwait(false);
                return;
            }

            var folder = await _googleClientService
                .GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            var reportCashImport = (List<string[]>) await _reportGateway
                .GetCashImportByDateAsync(batchReport.ReportStartDate.Value, batchReport.ReportEndDate.Value).ConfigureAwait(false);

            var fileName = $"Cash_Import_{batchReport.ReportStartDate.Value.ToString("ddMMyyyy")}_{batchReport.ReportEndDate.Value.ToString("ddMMyyyy")}_{batchReport.Id}.csv";

            await _googleClientService
                .UploadCsvFile(reportCashImport, fileName, googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            System.Threading.Thread.Sleep(_sleepDuration);

            var file = await _googleClientService
                .GetFileByNameInDriveAsync(googleFileSetting.GoogleIdentifier, fileName)
                .ConfigureAwait(false);

            var fileLink = $"https://drive.google.com/file/d/{file.Id}";
            await _batchReportGateway.SetStatusAsync(batchReport.Id, fileLink, true).ConfigureAwait(false);
        }

        private async Task CreateHousingBenefitAcademyReport(BatchReportDomain batchReport)
        {
            var googleFileSetting = await GetGoogleFileSetting(ReportHousingBenefitAcademyLabel).ConfigureAwait(false);
            if (googleFileSetting == null)
            {
                LoggingHandler.LogInfo($"Output folder not found");
                await _batchReportGateway.SetStatusAsync(batchReport.Id, "Output folder not found", false).ConfigureAwait(false);
                return;
            }

            var folder = await _googleClientService
                .GetFilesInDriveAsync(googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            var reportCashImport = (List<string[]>) await _reportGateway
                .GetHousingBenefitAcademyByYearAsync(batchReport.ReportYear.Value).ConfigureAwait(false);

            var fileName = $"HB_Academy_{batchReport.ReportYear.Value}_{batchReport.Id}.csv";

            await _googleClientService
                .UploadCsvFile(reportCashImport, fileName, googleFileSetting.GoogleIdentifier)
                .ConfigureAwait(false);

            System.Threading.Thread.Sleep(_sleepDuration);

            var file = await _googleClientService
                .GetFileByNameInDriveAsync(googleFileSetting.GoogleIdentifier, fileName)
                .ConfigureAwait(false);

            var fileLink = $"https://drive.google.com/file/d/{file.Id}";
            await _batchReportGateway.SetStatusAsync(batchReport.Id, fileLink, true).ConfigureAwait(false);
        }
    }
}
