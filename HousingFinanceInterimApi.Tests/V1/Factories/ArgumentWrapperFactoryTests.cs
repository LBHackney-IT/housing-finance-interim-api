using System;
using FluentAssertions;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Factories
{
    public class ArgumentWrapperFactoryTests
    {
        [Fact]
        public void ArgumentWrapperFactoryThrowsWhilstMappingToGetPRNTransactionFiltersWrapperWhenBatchReportIsNull()
        {
            // arrange
            var pulledBatchReportEvent = null as BatchReportDomain;

            var expectedErrorMsg = "Batch Report event is missing.";

            // act
            Action mapBatchReportToFilterWrapper = () => ArgumentWrapperFactory.ExtractPRNTransactionArgs(pulledBatchReportEvent);

            // assert
            mapBatchReportToFilterWrapper.Should().Throw<ArgumentException>().WithMessage(expectedErrorMsg);
        }

        [Fact]
        public void ArgumentWrapperFactoryCorrectlyMapsBatchReportToGetPRNTransactionFiltersWrapper()
        {
            // arrange
            var pulledBatchReportEvent = RandomGen.Create<BatchReportDomain>();

            // act
            var extractedFilterArgs = ArgumentWrapperFactory.ExtractPRNTransactionArgs(pulledBatchReportEvent);

            // assert
            extractedFilterArgs.RentGroup.Should().Be(pulledBatchReportEvent.RentGroup);
            extractedFilterArgs.FinancialYear.Should().Be(pulledBatchReportEvent.ReportYear.Value);
            extractedFilterArgs.StartWeekOrMonth.Should().Be(pulledBatchReportEvent.ReportStartWeekOrMonth.Value);
            extractedFilterArgs.EndWeekOrMonth.Should().Be(pulledBatchReportEvent.ReportEndWeekOrMonth.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ArgumentWrapperFactoryThrowsUponMappingToGetPRNTransactionFiltersWrapperWhenRentGroupIsMissing(string testRentGroup)
        {
            // arrange
            var pulledBatchReportEvent = RandomGen.Create<BatchReportDomain>();
            pulledBatchReportEvent.RentGroup = testRentGroup;

            var expectedErrorMsg = $"When requesting {pulledBatchReportEvent.ReportName} report, the Rent Group filter must be provided.";

            // act
            Action mapBatchReportToFilterWrapper = () => ArgumentWrapperFactory.ExtractPRNTransactionArgs(pulledBatchReportEvent);

            // assert
            mapBatchReportToFilterWrapper.Should().Throw<ArgumentException>().WithMessage(expectedErrorMsg);
        }

        [Fact]
        public void ArgumentWrapperFactoryThrowsUponMappingToGetPRNTransactionFiltersWrapperWhenReportYearIsMissing()
        {
            // arrange
            var pulledBatchReportEvent = RandomGen.Create<BatchReportDomain>();
            pulledBatchReportEvent.ReportYear = null;

            // act
            Action mapBatchReportToFilterWrapper = () => ArgumentWrapperFactory.ExtractPRNTransactionArgs(pulledBatchReportEvent);

            // assert
            mapBatchReportToFilterWrapper.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ArgumentWrapperFactoryThrowsUponMappingToGetPRNTransactionFiltersWrapperWhenReportStartWeekOrMonthIsMissing()
        {
            // arrange
            var pulledBatchReportEvent = RandomGen.Create<BatchReportDomain>();
            pulledBatchReportEvent.ReportStartWeekOrMonth = null;

            // act
            Action mapBatchReportToFilterWrapper = () => ArgumentWrapperFactory.ExtractPRNTransactionArgs(pulledBatchReportEvent);

            // assert
            mapBatchReportToFilterWrapper.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ArgumentWrapperFactoryThrowsUponMappingToGetPRNTransactionFiltersWrapperWhenReportEndWeekOrMonthIsMissing()
        {
            // arrange
            var pulledBatchReportEvent = RandomGen.Create<BatchReportDomain>();
            pulledBatchReportEvent.ReportEndWeekOrMonth = null;

            // act
            Action mapBatchReportToFilterWrapper = () => ArgumentWrapperFactory.ExtractPRNTransactionArgs(pulledBatchReportEvent);

            // assert
            mapBatchReportToFilterWrapper.Should().Throw<InvalidOperationException>();
        }
    }
}
