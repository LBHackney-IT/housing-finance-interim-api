using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class BatchReportAccountBalanceFactory
    {
        public static BatchReportAccountBalanceDomain ToDomain(this BatchReportAccountBalance batchReportAccountBalance)
        {
            if (batchReportAccountBalance == null)
                return null;

            return new BatchReportAccountBalanceDomain
            {
                Id = batchReportAccountBalance.Id,
                RentGroup = batchReportAccountBalance.RentGroup,
                ReportDate = batchReportAccountBalance.ReportDate,
                Link = batchReportAccountBalance.Link,
                StartTime = batchReportAccountBalance.StartTime,
                EndTime = batchReportAccountBalance.EndTime,
                IsSuccess = batchReportAccountBalance.IsSuccess
            };
        }

        public static List<BatchReportAccountBalanceDomain> ToDomain(
            this ICollection<BatchReportAccountBalance> batchReportAccountBalances)
        {
            return batchReportAccountBalances?.Select(b => b.ToDomain()).ToList();
        }

        public static BatchReportAccountBalanceDomain ToDomain(this BatchReportAccountBalanceRequest batchReportAccountBalance)
        {
            if (batchReportAccountBalance == null)
                return null;

            return new BatchReportAccountBalanceDomain
            {
                RentGroup = batchReportAccountBalance.RentGroup,
                ReportDate = batchReportAccountBalance.ReportDate
            };
        }

        public static List<BatchReportAccountBalanceDomain> ToDomain(
            this ICollection<BatchReportAccountBalanceRequest> batchReportAccountBalances)
        {
            return batchReportAccountBalances?.Select(b => b.ToDomain()).ToList();
        }

        public static BatchReportAccountBalance ToDatabase(this BatchReportAccountBalanceDomain batchReportAccountBalance)
        {
            if (batchReportAccountBalance == null)
                return null;

            return new BatchReportAccountBalance
            {
                Id = batchReportAccountBalance.Id,
                RentGroup = batchReportAccountBalance.RentGroup,
                ReportDate = batchReportAccountBalance.ReportDate,
                Link = batchReportAccountBalance.Link,
                StartTime = batchReportAccountBalance.StartTime,
                EndTime = batchReportAccountBalance.EndTime,
                IsSuccess = batchReportAccountBalance.IsSuccess
            };
        }

        public static List<BatchReportAccountBalance> ToDatabase(
            this ICollection<BatchReportAccountBalanceDomain> batchReportAccountBalances)
        {
            return batchReportAccountBalances?.Select(b => b.ToDatabase()).ToList();
        }

        //public static BatchReportAccountBalanceResponse ToResponse(this BatchReportAccountBalanceDomain batchReportAccountBalance)
        //{
        //    if (BatchReportAccountBalance == null)
        //        return null;

        //    return new BatchReportAccountBalanceResponse
        //    {
        //        BatchId = BatchReportAccountBalance.Id,
        //        ProcessName = BatchReportAccountBalance.Type,
        //        StartTime = BatchReportAccountBalance.StartTime,
        //        EndTime = BatchReportAccountBalance.EndTime,
        //        Errors = BatchReportAccountBalance.BatchReportAccountBalanceErrors.ToResponse()
        //    };
        //}

        //public static List<BatchReportAccountBalanceResponse> ToResponse(
        //    this ICollection<BatchReportAccountBalanceDomain> BatchReportAccountBalances)
        //{
        //    return BatchReportAccountBalances?.Select(b => b.ToResponse()).ToList();
        //}
    }
}
