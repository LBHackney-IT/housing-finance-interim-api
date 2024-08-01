using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.AspNetCore.Connections;
using Moq;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Gateways
{
    public class ReportGatewayTests
    {
        private readonly Mock<DatabaseContext> _mockHFSDatabaseContext;
        private readonly IReportGateway _classUnderTest;

        public ReportGatewayTests()
        {
            _mockHFSDatabaseContext = new Mock<DatabaseContext>();
            _classUnderTest = new ReportGateway(_mockHFSDatabaseContext.Object);
        }

        #region Account Balance
        [Fact]
        public async Task ReportGatewayAccountBalanceMethodReturnsTheTableDataThatItHasReceivedFromTheHFSDatabaseContext()
        {
            // arrange
            DateTime reportDate = RandomGen.DateTimeBetween();
            string rentGroup = RandomGen.String2(3);

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetReportAccountBalance(It.IsAny<DateTime>(), It.IsAny<string>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            var tableDataFromGW = await _classUnderTest
                .GetReportAccountBalanceAsync(reportDate, rentGroup)
                .ConfigureAwait(false);

            // assert
            tableDataFromGW.Should().IsSameOrEqualTo(dbContextResult);
        }

        [Fact]
        public async Task ReportGatewayAccountBalanceMethodThrowsWhenHFSDatabaseContextThrows()
        {
            // arrange
            DateTime reportDate = RandomGen.DateTimeBetween();
            string rentGroup = RandomGen.String2(3);

            var errorMessage = "An existing connection was forcibly closed by the remote host.";
            var dbContextResult = new ConnectionResetException(errorMessage);

            _mockHFSDatabaseContext
                .Setup(g => g.GetReportAccountBalance(
                    It.IsAny<DateTime>(),
                    It.IsAny<string>()
                ))
                .ThrowsAsync(dbContextResult);

            // act
            Func<Task> getReportDataGWCall = async () => await _classUnderTest
                .GetReportAccountBalanceAsync(reportDate, rentGroup)
                .ConfigureAwait(false);

            // assert
            await getReportDataGWCall.Should().ThrowAsync<ConnectionResetException>().WithMessage(errorMessage).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReportGatewayAccountBalanceMethodCallsTheHFSDatabaseContextAccountBalanceMethodWithExpectedParameters()
        {
            // arrange
            DateTime reportDate = RandomGen.DateTimeBetween();
            string rentGroup = RandomGen.String2(3);

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetReportAccountBalance(It.IsAny<DateTime>(), It.IsAny<string>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            await _classUnderTest
                .GetReportAccountBalanceAsync(reportDate, rentGroup)
                .ConfigureAwait(false);

            // assert
            _mockHFSDatabaseContext
                .Verify(
                    g => g.GetReportAccountBalance(
                        It.Is<DateTime>(d => d == reportDate),
                        It.Is<string>(t => t == rentGroup)
                    ),
                    Times.Once
                );
        }
        #endregion

        #region Charges
        // By Year and RentGroup
        [Fact]
        public async Task ReportGatewayChargesByYearAndRentGroupMethodReturnsTheTableDataThatItHasReceivedFromTheHFSDatabaseContext()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();
            string rentGroup = RandomGen.String2(3);

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetChargesByYearAndRentGroupAsync(It.IsAny<int>(), It.IsAny<string>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            var tableDataFromGW = await _classUnderTest
                .GetChargesByYearAndRentGroupAsync(financialYear, rentGroup)
                .ConfigureAwait(false);

            // assert
            tableDataFromGW.Should().IsSameOrEqualTo(dbContextResult);
        }

        [Fact]
        public async Task ReportGatewayChargesByYearAndRentGroupMethodThrowsWhenHFSDatabaseContextThrows()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();
            string rentGroup = RandomGen.String2(3);

            var errorMessage = "An existing connection was forcibly closed by the remote host.";
            var dbContextResult = new ConnectionResetException(errorMessage);

            _mockHFSDatabaseContext
                .Setup(g => g.GetChargesByYearAndRentGroupAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ))
                .ThrowsAsync(dbContextResult);

            // act
            Func<Task> getReportDataGWCall = async () => await _classUnderTest
                .GetChargesByYearAndRentGroupAsync(financialYear, rentGroup)
                .ConfigureAwait(false);

            // assert
            await getReportDataGWCall.Should().ThrowAsync<ConnectionResetException>().WithMessage(errorMessage).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReportGatewayChargesByYearAndRentGroupMethodCallsTheHFSDatabaseContextChargesByYearAndRentGroupMethodWithExpectedParameters()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();
            string rentGroup = RandomGen.String2(3);

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetChargesByYearAndRentGroupAsync(It.IsAny<int>(), It.IsAny<string>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            await _classUnderTest
                .GetChargesByYearAndRentGroupAsync(financialYear, rentGroup)
                .ConfigureAwait(false);

            // assert
            _mockHFSDatabaseContext
                .Verify(
                    g => g.GetChargesByYearAndRentGroupAsync(
                        It.Is<int>(y => y == financialYear),
                        It.Is<string>(t => t == rentGroup)
                    ),
                    Times.Once
                );
        }

        // By Year and GroupType
        [Fact]
        public async Task ReportGatewayChargesByGroupTypeMethodReturnsTheTableDataThatItHasReceivedFromTheHFSDatabaseContext()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();
            string groupType = RandomGen.String2(8);

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetChargesByGroupType(It.IsAny<int>(), It.IsAny<string>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            var tableDataFromGW = await _classUnderTest
                .GetChargesByGroupTypeAsync(financialYear, groupType)
                .ConfigureAwait(false);

            // assert
            tableDataFromGW.Should().IsSameOrEqualTo(dbContextResult);
        }

        [Fact]
        public async Task ReportGatewayChargesByGroupTypeMethodThrowsWhenHFSDatabaseContextThrows()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();
            string groupType = RandomGen.String2(8);

            var errorMessage = "An existing connection was forcibly closed by the remote host.";
            var dbContextResult = new ConnectionResetException(errorMessage);

            _mockHFSDatabaseContext
                .Setup(g => g.GetChargesByGroupType(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ))
                .ThrowsAsync(dbContextResult);

            // act
            Func<Task> getReportDataGWCall = async () => await _classUnderTest
                .GetChargesByGroupTypeAsync(financialYear, groupType)
                .ConfigureAwait(false);

            // assert
            await getReportDataGWCall.Should().ThrowAsync<ConnectionResetException>().WithMessage(errorMessage).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReportGatewayChargesByGroupTypeMethodCallsTheHFSDatabaseContextChargesByGroupTypeMethodWithExpectedParameters()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();
            string groupType = RandomGen.String2(8);

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetChargesByGroupType(It.IsAny<int>(), It.IsAny<string>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            await _classUnderTest
                .GetChargesByGroupTypeAsync(financialYear, groupType)
                .ConfigureAwait(false);

            // assert
            _mockHFSDatabaseContext
                .Verify(
                    g => g.GetChargesByGroupType(
                        It.Is<int>(y => y == financialYear),
                        It.Is<string>(t => t == groupType)
                    ),
                    Times.Once
                );
        }

        // By Year
        [Fact]
        public async Task ReportGatewayChargesByYearMethodReturnsTheTableDataThatItHasReceivedFromTheHFSDatabaseContext()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetChargesByYear(It.IsAny<int>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            var tableDataFromGW = await _classUnderTest
                .GetChargesByYearAsync(financialYear)
                .ConfigureAwait(false);

            // assert
            tableDataFromGW.Should().IsSameOrEqualTo(dbContextResult);
        }

        [Fact]
        public async Task ReportGatewayChargesByYearMethodThrowsWhenHFSDatabaseContextThrows()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();

            var errorMessage = "An existing connection was forcibly closed by the remote host.";
            var dbContextResult = new ConnectionResetException(errorMessage);

            _mockHFSDatabaseContext
                .Setup(g => g.GetChargesByYear(
                    It.IsAny<int>()
                ))
                .ThrowsAsync(dbContextResult);

            // act
            Func<Task> getReportDataGWCall = async () => await _classUnderTest
                .GetChargesByYearAsync(financialYear)
                .ConfigureAwait(false);

            // assert
            await getReportDataGWCall.Should().ThrowAsync<ConnectionResetException>().WithMessage(errorMessage).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReportGatewayChargesByYearMethodCallsTheHFSDatabaseContextChargesByYearMethodWithExpectedParameters()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetChargesByYear(It.IsAny<int>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            await _classUnderTest
                .GetChargesByYearAsync(financialYear)
                .ConfigureAwait(false);

            // assert
            _mockHFSDatabaseContext
                .Verify(
                    g => g.GetChargesByYear(
                        It.Is<int>(y => y == financialYear)
                    ),
                    Times.Once
                );
        }
        #endregion

        #region Itemised Transactions
        [Fact]
        public async Task ReportGatewayItemisedTransactionsMethodReturnsTheTableDataThatItHasReceivedFromTheHFSDatabaseContext()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();
            string transactionType = RandomGen.String2(3);

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetItemisedTransactionsByYearAndTransactionTypeAsync(It.IsAny<int>(), It.IsAny<string>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            var tableDataFromGW = await _classUnderTest
                .GetItemisedTransactionsByYearAndTransactionTypeAsync(financialYear, transactionType)
                .ConfigureAwait(false);

            // assert
            tableDataFromGW.Should().IsSameOrEqualTo(dbContextResult);
        }

        [Fact]
        public async Task ReportGatewayItemisedTransactionsMethodThrowsWhenHFSDatabaseContextThrows()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();
            string transactionType = RandomGen.String2(3);

            var errorMessage = "An existing connection was forcibly closed by the remote host.";
            var dbContextResult = new ConnectionResetException(errorMessage);

            _mockHFSDatabaseContext
                .Setup(g => g.GetItemisedTransactionsByYearAndTransactionTypeAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ))
                .ThrowsAsync(dbContextResult);

            // act
            Func<Task> getReportDataGWCall = async () => await _classUnderTest
                .GetItemisedTransactionsByYearAndTransactionTypeAsync(financialYear, transactionType)
                .ConfigureAwait(false);

            // assert
            await getReportDataGWCall.Should().ThrowAsync<ConnectionResetException>().WithMessage(errorMessage).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReportGatewayItemisedTransactionsMethodCallsTheHFSDatabaseContextItemisedTransactionsMethodWithExpectedParameters()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();
            string transactionType = RandomGen.String2(3);

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetItemisedTransactionsByYearAndTransactionTypeAsync(It.IsAny<int>(), It.IsAny<string>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            await _classUnderTest
                .GetItemisedTransactionsByYearAndTransactionTypeAsync(financialYear, transactionType)
                .ConfigureAwait(false);

            // assert
            _mockHFSDatabaseContext
                .Verify(
                    g => g.GetItemisedTransactionsByYearAndTransactionTypeAsync(
                        It.Is<int>(y => y == financialYear),
                        It.Is<string>(t => t == transactionType)
                    ),
                    Times.Once
                );
        }
        #endregion

        #region Cash Suspense
        [Fact]
        public async Task ReportGatewayCashSuspenseAccountByYearMethodReturnsTheTableDataThatItHasReceivedFromTheHFSDatabaseContext()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();
            string suspenseAccountType = RandomGen.String2(8);

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetCashSuspenseAccountByYearAsync(It.IsAny<int>(), It.IsAny<string>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            var tableDataFromGW = await _classUnderTest
                .GetCashSuspenseAccountByYearAsync(financialYear, suspenseAccountType)
                .ConfigureAwait(false);

            // assert
            tableDataFromGW.Should().IsSameOrEqualTo(dbContextResult);
        }

        [Fact]
        public async Task ReportGatewayCashSuspenseAccountByYearMethodThrowsWhenHFSDatabaseContextThrows()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();
            string suspenseAccountType = RandomGen.String2(8);

            var errorMessage = "An existing connection was forcibly closed by the remote host.";
            var dbContextResult = new ConnectionResetException(errorMessage);

            _mockHFSDatabaseContext
                .Setup(g => g.GetCashSuspenseAccountByYearAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()
                ))
                .ThrowsAsync(dbContextResult);

            // act
            Func<Task> getReportDataGWCall = async () => await _classUnderTest
                .GetCashSuspenseAccountByYearAsync(financialYear, suspenseAccountType)
                .ConfigureAwait(false);

            // assert
            await getReportDataGWCall.Should().ThrowAsync<ConnectionResetException>().WithMessage(errorMessage).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReportGatewayCashSuspenseAccountByYearMethodCallsTheHFSDatabaseContextCashSuspenseAccountByYearMethodWithExpectedParameters()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();
            string suspenseAccountType = RandomGen.String2(8);

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetCashSuspenseAccountByYearAsync(It.IsAny<int>(), It.IsAny<string>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            await _classUnderTest
                .GetCashSuspenseAccountByYearAsync(financialYear, suspenseAccountType)
                .ConfigureAwait(false);

            // assert
            _mockHFSDatabaseContext
                .Verify(
                    g => g.GetCashSuspenseAccountByYearAsync(
                        It.Is<int>(y => y == financialYear),
                        It.Is<string>(t => t == suspenseAccountType)
                    ),
                    Times.Once
                );
        }
        #endregion

        #region Cash Import
        [Fact]
        public async Task ReportGatewayCashImportByDateMethodReturnsTheTableDataThatItHasReceivedFromTheHFSDatabaseContext()
        {
            // arrange
            DateTime startDate = RandomGen.DateTimeBetween();
            DateTime endDate = RandomGen.DateTimeBetween();

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetCashImportByDateAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            var tableDataFromGW = await _classUnderTest
                .GetCashImportByDateAsync(startDate, endDate)
                .ConfigureAwait(false);

            // assert
            tableDataFromGW.Should().IsSameOrEqualTo(dbContextResult);
        }

        [Fact]
        public async Task ReportGatewayCashImportByDateMethodThrowsWhenHFSDatabaseContextThrows()
        {
            // arrange
            DateTime startDate = RandomGen.DateTimeBetween();
            DateTime endDate = RandomGen.DateTimeBetween();

            var errorMessage = "An existing connection was forcibly closed by the remote host.";
            var dbContextResult = new ConnectionResetException(errorMessage);

            _mockHFSDatabaseContext
                .Setup(g => g.GetCashImportByDateAsync(
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()
                ))
                .ThrowsAsync(dbContextResult);

            // act
            Func<Task> getReportDataGWCall = async () => await _classUnderTest
                .GetCashImportByDateAsync(startDate, endDate)
                .ConfigureAwait(false);

            // assert
            await getReportDataGWCall.Should().ThrowAsync<ConnectionResetException>().WithMessage(errorMessage).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReportGatewayCashImportByDateMethodCallsTheHFSDatabaseContextCashImportByDateMethodWithExpectedParameters()
        {
            // arrange
            DateTime startDate = RandomGen.DateTimeBetween();
            DateTime endDate = RandomGen.DateTimeBetween();

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetCashImportByDateAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            await _classUnderTest
                .GetCashImportByDateAsync(startDate, endDate)
                .ConfigureAwait(false);

            // assert
            _mockHFSDatabaseContext
                .Verify(
                    g => g.GetCashImportByDateAsync(
                        It.Is<DateTime>(s => s == startDate),
                        It.Is<DateTime>(e => e == endDate)
                    ),
                    Times.Once
                );
        }
        #endregion

        #region Housing Benefit Academy
        [Fact]
        public async Task ReportGatewayHousingBenefitAcademyByYearMethodReturnsTheTableDataThatItHasReceivedFromTheHFSDatabaseContext()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetHousingBenefitAcademyByYear(It.IsAny<int>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            var tableDataFromGW = await _classUnderTest
                .GetHousingBenefitAcademyByYearAsync(financialYear)
                .ConfigureAwait(false);

            // assert
            tableDataFromGW.Should().IsSameOrEqualTo(dbContextResult);
        }

        [Fact]
        public async Task ReportGatewayHousingBenefitAcademyByYearMethodThrowsWhenHFSDatabaseContextThrows()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();

            var errorMessage = "An existing connection was forcibly closed by the remote host.";
            var dbContextResult = new ConnectionResetException(errorMessage);

            _mockHFSDatabaseContext
                .Setup(g => g.GetHousingBenefitAcademyByYear(
                    It.IsAny<int>()
                ))
                .ThrowsAsync(dbContextResult);

            // act
            Func<Task> getReportDataGWCall = async () => await _classUnderTest
                .GetHousingBenefitAcademyByYearAsync(financialYear)
                .ConfigureAwait(false);

            // assert
            await getReportDataGWCall.Should().ThrowAsync<ConnectionResetException>().WithMessage(errorMessage).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReportGatewayHousingBenefitAcademyByYearMethodCallsTheHFSDatabaseContextHousingBenefitAcademyByYearMethodWithExpectedParameters()
        {
            // arrange
            int financialYear = RandomGen.WholeNumber();

            var dbContextResult = new List<string[]>()
            {
                new string[] { "some_header", "other_header" }
            };

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetHousingBenefitAcademyByYear(It.IsAny<int>())
                )
                .ReturnsAsync(dbContextResult);

            // act
            await _classUnderTest
                .GetHousingBenefitAcademyByYearAsync(financialYear)
                .ConfigureAwait(false);

            // assert
            _mockHFSDatabaseContext
                .Verify(
                    g => g.GetHousingBenefitAcademyByYear(
                        It.Is<int>(y => y == financialYear)
                    ),
                    Times.Once
                );
        }
        #endregion
    }
}
