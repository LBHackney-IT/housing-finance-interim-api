using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Request;
using Microsoft.EntityFrameworkCore;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    public interface IDatabaseContext
    {
        Task<IList<OperatingBalance>> GetOperatingBalancesAsync(DateTime? startDate, DateTime? endDate, int startWeek, int startYear, int endWeek, int endYear);
        Task<IList<Payment>> GetPaymentsAsync(string tenancyAgreementRef, string rentAccount, string householdRef, int count, string order);
        Task UpdateAssetDetails(UpdateAssetDetailsQuery query, UpdateAssetDetailsRequest request);
        Task<IList<Tenancy>> GetTenanciesAsync(string tenancyAgreementRef, string rentAccount, string householdRef);
        Task<IList<TenancyTransaction>> GetTenancyTransactionsAsync(string tenancyAgreementRef, string rentAccount, string householdRef, int count, string order);
        Task<IList<TenancyTransaction>> GetTenancyTransactionsByDateAsync(string tenancyAgreementRef, string rentAccount, string householdRef, DateTime startDate, DateTime endDate);
        Task DeleteRentBreakdowns();
        Task DeleteCurrentRentPositions();
        Task DeleteServiceChargePaymentsReceived();
        Task DeleteLeaseholdAccounts();
        Task DeleteGarages();
        Task DeleteOtherHRA();
        Task GenerateSpreadsheetTransaction();
        Task RefreshManageArrearsMember();
        Task RefreshManageArrearsProperty();
        Task<List<SuspenseTransaction>> GetCashSuspenseTransactions();
        Task<List<SuspenseTransaction>> GetHousingBenefitSuspenseTransactions();
        Task<IList<Transaction>> GetTransactionsAsync(DateTime? startDate, DateTime? endDate);
        Task<IList<PRNTransactionEntity>> GetPRNTransactionsByRentGroupAsync(string rentGroup, int financialYear, int startWeekOrMonth, int endWeekOrMonth);
        Task RefreshTenancyAgreementTables(long batchLogId);
        Task LoadCashFiles();
        Task LoadHousingFiles();
        Task LoadDirectDebit(long batchLogId);
        Task LoadCharges();
        Task LoadActionDiary();
        Task LoadCashFileTransactions();
        Task LoadChargesTransactions(int @processingYear);
        Task LoadHousingFileTransactions();
        Task LoadDirectDebitTransactions();
        Task LoadAdjustmentTransactions();
        Task LoadDirectDebitHistory(DateTime? processingDate);
        Task LoadCashSuspenseTransactions();
        Task LoadHousingBenefitSuspenseTransactions();
        Task LoadChargesHistory(int @processingYear);
        Task CreateCashFileSuspenseAccountTransaction(long id, string newRentAccount);
        Task CreateHousingFileSuspenseAccountTransaction(long id, string newRentAccount);
        Task TruncateTenancyAgreementAuxiliary();
        Task TruncateDirectDebitAuxiliary();
        Task TruncateSuspenseTransactionAuxiliary();
        Task TruncateChargesAuxiliary();
        Task TruncateActionDiaryAuxiliary();
        Task TruncateAdjustmentsAuxiliary();
        Task UpdateCurrentBalance();
        Task GenerateOperatingBalance();
        Task RefreshManageArrearsTenancyAgreement();
        Task<List<string[]>> GetRentPosition();

        //REPORTS
        Task<List<string[]>> GetCashSuspenseAccountByYearAsync(int year, string suspenseAccountType);
        Task<List<string[]>> GetCashImportByDateAsync(DateTime startDate, DateTime endDate);
        Task<List<string[]>> GetChargesByYearAndRentGroupAsync(int year, string rentGroup);
        Task<List<string[]>> GetChargesByGroupType(int year, string type);
        Task<List<string[]>> GetChargesByYear(int year);
        Task<List<string[]>> GetItemisedTransactionsByYearAndTransactionTypeAsync(int year, string transactionType);
        Task<List<string[]>> GetHousingBenefitAcademyByYear(int year);
        Task<IList<string[]>> GetReportAccountBalance(DateTime reportDate, string rentGroup);

        public DbSet<BatchLog> BatchLogs { get; set; }
        public DbSet<BatchLogError> BatchLogErrors { get; set; }
        public DbSet<ChargesBatchYear> ChargesBatchYears { get; set; }
        public DbSet<MAProperty> MAProperty { get; set; }
        public DbSet<UHProperty> UHProperty { get; set; }
        public DbSet<GoogleFileSetting> GoogleFileSettings { get; set; }
        public DbSet<UPCashDumpFileName> UpCashDumpFileNames { get; set; }
        public DbSet<UPCashDump> UpCashDumps { get; set; }
        public DbSet<UPCashLoad> UpCashLoads { get; set; }
        public DbSet<SSMiniTransaction> SSMiniTransactions { get; set; }
        public DbSet<UPHousingCashDumpFileName> UpHousingCashDumpFileNames { get; set; }
        public DbSet<UPHousingCashDump> UpHousingCashDumps { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<RentBreakdown> RentBreakdowns { get; set; }
        public DbSet<CurrentRentPosition> CurrentRentPositions { get; set; }
        public DbSet<ServiceChargePaymentsReceived> ServiceChargesPaymentsReceived { get; set; }
        public DbSet<LeaseholdAccount> LeaseholdAccounts { get; set; }
        public DbSet<Garage> Garages { get; set; }
        public DbSet<OtherHRA> OtherHRA { get; set; }
        public DbSet<SuspenseTransaction> SuspenseTransactions { get; set; }
        public DbSet<SuspenseTransactionAux> SuspenseTransactionsAux { get; set; }
        public DbSet<ReportCashSuspenseAccount> ReportCashSuspenseAccounts { get; set; }
        public DbSet<ReportCashImport> ReportCashImports { get; set; }
        public DbSet<BatchReport> BatchReports { get; set; }
        public DbSet<ReportAccountBalance> ReportAccountBalances { get; set; }
    }
}
