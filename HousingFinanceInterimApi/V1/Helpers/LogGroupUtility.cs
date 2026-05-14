using System.Collections.Generic;

namespace HousingFinanceInterimApi.V1.Helpers
{
    public static class LogGroupUtility
    {
        public static List<string> GetLogGroups(string environmentName)
        {
            environmentName = environmentName ?? "production";

            return new List<string>
            {
                "hfs-nightly-jobs-charges-ingest-ecs-task-logs",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-load-tenagree",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-cash-file-trans",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-check-cash-files",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-direct-debit",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-direct-debit-trans",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-direct-debit-trans-dem",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-cash-file",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-adjustments-trans",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-action-diary",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-susp-cash",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-susp-hb",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-housing-file",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-housing-file-trans",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-refresh-cur-bal",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-refresh-op-bal",
                $"/aws/lambda/housing-finance-interim-api-{environmentName}-rent-position",
            };
        }
    }
}
