using System;
using System.Collections.Generic;
using System.Threading;
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

        DbSet<NightlyProcessLog> NightlyProcessLogs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
