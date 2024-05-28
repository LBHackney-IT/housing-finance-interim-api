using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class BatchReport
    {
        public static BatchReportDomain ToDomain(this Infrastructure.BatchReport batchReport)
        {
            if (batchReport == null)
                return null;

            return new BatchReportDomain
            {
                Id = batchReport.Id,
                ReportName = batchReport.ReportName,
                RentGroup = batchReport.RentGroup,
                Group = batchReport.Group,
                TransactionType = batchReport.TransactionType,
                ReportStartDate = batchReport.ReportStartDate,
                ReportEndDate = batchReport.ReportEndDate,
                ReportYear = batchReport.ReportYear,
                ReportDate = batchReport.ReportDate,
                ReportStartWeekOrMonth = batchReport.ReportStartWeekOrMonth,
                ReportEndWeekOrMonth = batchReport.ReportEndWeekOrMonth,
                Link = batchReport.Link,
                StartTime = batchReport.StartTime,
                EndTime = batchReport.EndTime,
                IsSuccess = batchReport.IsSuccess
            };
        }

        public static List<BatchReportDomain> ToDomain(
            this ICollection<Infrastructure.BatchReport> batchReportAccountBalances)
        {
            return batchReportAccountBalances?.Select(b => b.ToDomain()).ToList();
        }

        public static BatchReportDomain ToDomain(this BatchReportAccountBalanceRequest batchReportAccountBalance)
        {
            if (batchReportAccountBalance == null)
                return null;

            return new BatchReportDomain
            {
                RentGroup = batchReportAccountBalance.RentGroup,
                ReportDate = batchReportAccountBalance.ReportDate
            };
        }

        public static List<BatchReportDomain> ToDomain(
            this ICollection<BatchReportAccountBalanceRequest> batchReportAccountBalances)
        {
            return batchReportAccountBalances?.Select(b => b.ToDomain()).ToList();
        }

        public static BatchReportDomain ToDomain(this BatchReportChargesRequest batchReportCharge)
        {
            if (batchReportCharge == null)
                return null;

            return new BatchReportDomain
            {
                RentGroup = batchReportCharge.RentGroup,
                Group = batchReportCharge.Group,
                ReportYear = batchReportCharge.Year
            };
        }

        public static List<BatchReportDomain> ToDomain(
            this ICollection<BatchReportChargesRequest> batchReportCharges)
        {
            return batchReportCharges?.Select(b => b.ToDomain()).ToList();
        }

        #region Operating Balances by Rent Account
        public static BatchReportDomain ToDomain(this BatchReportOperatingBalancesByRentAccountRequest batchReportRequest)
        {
            if (batchReportRequest == null)
                return null;

            return new BatchReportDomain
            {
                RentGroup = batchReportRequest.RentGroup,
                ReportYear = batchReportRequest.FinancialYear,
                ReportStartWeekOrMonth = batchReportRequest.StartWeekOrMonth,
                ReportEndWeekOrMonth = batchReportRequest.EndWeekOrMonth
            };
        }

        public static BatchReportOperatingBalancesByRentAccountResponse ToReportOperatingBalancesByRentAccountResponse(this BatchReportDomain batchReport)
        {
            if (batchReport == null)
                return null;

            return new BatchReportOperatingBalancesByRentAccountResponse
            {
                Id = batchReport.Id,
                RentGroup = batchReport.RentGroup,
                FinancialYear = batchReport.ReportYear.Value,
                StartWeekOrMonth = batchReport.ReportStartWeekOrMonth.Value,
                EndWeekOrMonth = batchReport.ReportEndWeekOrMonth.Value,
                StartTime = batchReport.StartTime,
                EndTime = batchReport.EndTime,
                Link = batchReport.Link,
                IsSuccess = batchReport.IsSuccess
            };
        }
        #endregion
        #region Itemised Transactions
        public static BatchReportDomain ToDomain(this BatchReportItemisedTransactionRequest batchReportRequest)
        {
            if (batchReportRequest == null)
                return null;

            return new BatchReportDomain
            {
                ReportYear = batchReportRequest.Year,
                TransactionType = batchReportRequest.TransactionType
            };
        }

        public static BatchReportItemisedTransactionResponse ToReportItemisedTransactionResponse(this BatchReportDomain batchReport)
        {
            if (batchReport == null)
                return null;

            return new BatchReportItemisedTransactionResponse
            {
                Id = batchReport.Id,
                Year = batchReport.ReportYear.Value,
                TransactionType = batchReport.TransactionType,
                Link = batchReport.Link,
                StartTime = batchReport.StartTime,
                EndTime = batchReport.EndTime,
                IsSuccess = batchReport.IsSuccess
            };
        }

        public static List<BatchReportItemisedTransactionResponse> ToReportItemisedTransactionsResponse(
            this ICollection<BatchReportDomain> batchReports)
        {
            return batchReports?.Select(b => b.ToReportItemisedTransactionResponse()).ToList();
        }
        #endregion

        public static BatchReportDomain ToDomain(this BatchReportCashSuspenseRequest batchReportCashSuspense)
        {
            if (batchReportCashSuspense == null)
                return null;

            return new BatchReportDomain
            {
                Group = batchReportCashSuspense.SuspenseAccountType,
                ReportYear = batchReportCashSuspense.Year
            };
        }

        public static List<BatchReportDomain> ToDomain(
            this ICollection<BatchReportCashSuspenseRequest> batchReportCashSuspenses)
        {
            return batchReportCashSuspenses?.Select(b => b.ToDomain()).ToList();
        }

        public static BatchReportDomain ToDomain(this BatchReportCashImportRequest batchReportCashImport)
        {
            if (batchReportCashImport == null)
                return null;

            return new BatchReportDomain
            {
                ReportStartDate = batchReportCashImport.StartDate,
                ReportEndDate = batchReportCashImport.EndDate
            };
        }

        public static List<BatchReportDomain> ToDomain(
            this ICollection<BatchReportCashImportRequest> batchReportCashImports)
        {
            return batchReportCashImports?.Select(b => b.ToDomain()).ToList();
        }

        public static BatchReportDomain ToDomain(this BatchReportHousingBenefitAcademyRequest batchReportHousingBenefitAcademy)
        {
            if (batchReportHousingBenefitAcademy == null)
                return null;

            return new BatchReportDomain
            {
                ReportYear = batchReportHousingBenefitAcademy.Year
            };
        }

        public static List<BatchReportDomain> ToDomain(
            this ICollection<BatchReportHousingBenefitAcademyRequest> batchReportHousingBenefitAcademy)
        {
            return batchReportHousingBenefitAcademy?.Select(b => b.ToDomain()).ToList();
        }

        // TODO: this change belongs to the POST endpoint PR.
        public static Infrastructure.BatchReport ToDatabase(this BatchReportDomain batchReport)
        {
            if (batchReport == null)
                return null;

            return new Infrastructure.BatchReport
            {
                Id = batchReport.Id,
                ReportName = batchReport.ReportName,
                RentGroup = batchReport.RentGroup,
                Group = batchReport.Group,
                TransactionType = batchReport.TransactionType,
                ReportStartDate = batchReport.ReportStartDate,
                ReportEndDate = batchReport.ReportEndDate,
                ReportYear = batchReport.ReportYear,
                ReportDate = batchReport.ReportDate,
                Link = batchReport.Link,
                StartTime = batchReport.StartTime,
                EndTime = batchReport.EndTime,
                IsSuccess = batchReport.IsSuccess
            };
        }

        public static List<Infrastructure.BatchReport> ToDatabase(
            this ICollection<BatchReportDomain> batchReportAccountBalances)
        {
            return batchReportAccountBalances?.Select(b => b.ToDatabase()).ToList();
        }

        public static BatchReportAccountBalanceResponse ToReportAccountBalanceResponse(this BatchReportDomain batchReport)
        {
            if (batchReport == null)
                return null;

            return new BatchReportAccountBalanceResponse
            {
                Id = batchReport.Id,
                RentGroup = batchReport.RentGroup,
                ReportDate = batchReport.ReportDate.Value,
                Link = batchReport.Link,
                StartTime = batchReport.StartTime,
                EndTime = batchReport.EndTime,
                IsSuccess = batchReport.IsSuccess
            };
        }

        public static List<BatchReportAccountBalanceResponse> ToReportAccountBalanceResponse(
            this ICollection<BatchReportDomain> batchReports)
        {
            return batchReports?.Select(b => b.ToReportAccountBalanceResponse()).ToList();
        }

        public static BatchReportChargesResponse ToReportChargesResponse(this BatchReportDomain batchReport)
        {
            if (batchReport == null)
                return null;

            return new BatchReportChargesResponse
            {
                Id = batchReport.Id,
                Year = batchReport.ReportYear.Value,
                RentGroup = batchReport.RentGroup,
                Group = batchReport.Group,
                Link = batchReport.Link,
                StartTime = batchReport.StartTime,
                EndTime = batchReport.EndTime,
                IsSuccess = batchReport.IsSuccess
            };
        }

        public static List<BatchReportChargesResponse> ToReportChargesResponse(
            this ICollection<BatchReportDomain> batchReports)
        {
            return batchReports?.Select(b => b.ToReportChargesResponse()).ToList();
        }

        public static BatchReportCashSuspenseResponse ToReportCashSuspenseResponse(this BatchReportDomain batchReport)
        {
            if (batchReport == null)
                return null;

            return new BatchReportCashSuspenseResponse
            {
                Id = batchReport.Id,
                Year = batchReport.ReportYear.Value,
                SuspenseAccountType = batchReport.Group,
                Link = batchReport.Link,
                StartTime = batchReport.StartTime,
                EndTime = batchReport.EndTime,
                IsSuccess = batchReport.IsSuccess
            };
        }

        public static List<BatchReportCashSuspenseResponse> ToReportCashSuspenseResponse(
            this ICollection<BatchReportDomain> batchReports)
        {
            return batchReports?.Select(b => b.ToReportCashSuspenseResponse()).ToList();
        }

        public static BatchReportCashImportResponse ToReportCashImportResponse(this BatchReportDomain batchReport)
        {
            if (batchReport == null)
                return null;

            return new BatchReportCashImportResponse
            {
                Id = batchReport.Id,
                StartDate = batchReport.ReportStartDate.Value,
                EndDate = batchReport.ReportEndDate.Value,
                Link = batchReport.Link,
                StartTime = batchReport.StartTime,
                EndTime = batchReport.EndTime,
                IsSuccess = batchReport.IsSuccess
            };
        }

        public static List<BatchReportCashImportResponse> ToReportCashImportResponse(
            this ICollection<BatchReportDomain> batchReports)
        {
            return batchReports?.Select(b => b.ToReportCashImportResponse()).ToList();
        }

        public static BatchReportHousingBenefitAcademyResponse ToReportHousingBenefitAcademyResponse(this BatchReportDomain batchReport)
        {
            if (batchReport == null)
                return null;

            return new BatchReportHousingBenefitAcademyResponse
            {
                Id = batchReport.Id,
                Year = batchReport.ReportYear.Value,
                Link = batchReport.Link,
                StartTime = batchReport.StartTime,
                EndTime = batchReport.EndTime,
                IsSuccess = batchReport.IsSuccess
            };
        }

        public static List<BatchReportHousingBenefitAcademyResponse> ToReportHousingBenefitAcademyResponse(
            this ICollection<BatchReportDomain> batchReports)
        {
            return batchReports?.Select(b => b.ToReportHousingBenefitAcademyResponse()).ToList();
        }
    }
}
