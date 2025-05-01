using Xunit;
using HousingFinanceInterimApi.V1.Helpers;

namespace HousingFinanceInterimApi.Tests.V1.Helpers
{
    public class LogGroupUtilityTests
    {
        [Fact]
        public void GetLogGroups_ShouldReturnCorrectLogGroups_ForGivenEnvironment()
        {
            // Arrange
            var environmentName = "test";

            // Act
            var logGroups = LogGroupUtility.GetLogGroups(environmentName);

            // Assert
            Assert.Contains("/aws/lambda/housing-finance-interim-api-test-load-tenagree", logGroups);
            Assert.Contains("/aws/lambda/housing-finance-interim-api-test-cash-file-trans", logGroups);
            Assert.Contains("hfs-nightly-jobs-charges-ingest-ecs-task-logs", logGroups);
        }

        [Fact]
        public void GetLogGroups_ShouldDefaultToProduction_WhenEnvironmentNameIsNull()
        {
            // Act
            var logGroups = LogGroupUtility.GetLogGroups(null);

            // Assert
            Assert.Contains("/aws/lambda/housing-finance-interim-api-production-load-tenagree", logGroups);
            Assert.Contains("/aws/lambda/housing-finance-interim-api-production-cash-file-trans", logGroups);
            Assert.Contains("hfs-nightly-jobs-charges-ingest-ecs-task-logs", logGroups);
        }
    }
}
