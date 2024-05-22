using System;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Domain.ArgumentWrappers;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class ArgumentWrapperFactory
    {
        public static GetPRNTransactionsDomain ExtractPRNTransactionArgs(this BatchReportDomain batchReport)
        {
            if (batchReport is null)
                throw new ArgumentException("Batch Report event is missing.");

            var rentGroup = batchReport.RentGroup;

            if (string.IsNullOrWhiteSpace(rentGroup))
                throw new ArgumentException(
                    $"When requesting {batchReport.ReportName} report, the Rent Group filter must be provided."
                );

            return new GetPRNTransactionsDomain
            {
                RentGroup = rentGroup,
                // the following values are mandatory for this type of report, as such...
                // we want the '.Value' to throw implicitly when any of the values are null.
                FinancialYear = batchReport.ReportYear.Value,
                StartWeekOrMonth = batchReport.ReportStartWeekOrMonth.Value,
                EndWeekOrMonth = batchReport.ReportEndWeekOrMonth.Value,
            };
        }
    }
}
