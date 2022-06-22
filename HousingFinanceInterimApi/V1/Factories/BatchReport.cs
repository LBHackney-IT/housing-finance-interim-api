using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

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

        public static BatchReportAccountBalanceResponse ToResponse(this BatchReportDomain batchReport)
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

        public static List<BatchReportAccountBalanceResponse> ToResponse(
            this ICollection<BatchReportDomain> batchReports)
        {
            return batchReports?.Select(b => b.ToResponse()).ToList();
        }
    }
}
