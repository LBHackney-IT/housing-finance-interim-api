using System;
using System.Threading.Tasks;
using FluentAssertions;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Domain.ArgumentWrappers;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.AspNetCore.Connections;
using Moq;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Gateways
{
    public class TransactionGatewayTests
    {
        private readonly Mock<IDatabaseContext> _mockHFSDatabaseContext;
        private readonly ITransactionGateway _classUnderTest;

        public TransactionGatewayTests()
        {
            _mockHFSDatabaseContext = new Mock<IDatabaseContext>();
            _classUnderTest = new TransactionGateway(_mockHFSDatabaseContext.Object);
        }

        [Fact]
        public async Task GetPRNTransactionsGatewayMethodCallsApproapriateDbContextMethod()
        {
            // arrange
            var irrelDbContextReturn = RandomGen.CreateMany<PRNTransactionEntity>();

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetPRNTransactionsByRentGroupAsync(
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<int>()
                    )
                )
                .ReturnsAsync(irrelDbContextReturn);

            var irrelevantFilters = RandomGen.Create<GetPRNTransactionsDomain>();

            // act
            await _classUnderTest
                .GetPRNTransactions(irrelevantFilters)
                .ConfigureAwait(false);

            // assert
            _mockHFSDatabaseContext.Verify(
                c => c.GetPRNTransactionsByRentGroupAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetPRNTransactionsGatewayMethodCallsCorrectDbContextMethodWithCorrectParameters()
        {
            // arrange
            var irrelDbContextReturn = RandomGen.CreateMany<PRNTransactionEntity>();

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetPRNTransactionsByRentGroupAsync(
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<int>()
                    )
                )
                .ReturnsAsync(irrelDbContextReturn);

            var expectedTransactionFilters = RandomGen.Create<GetPRNTransactionsDomain>();

            // act
            await _classUnderTest
                .GetPRNTransactions(expectedTransactionFilters)
                .ConfigureAwait(false);

            // assert
            _mockHFSDatabaseContext.Verify(
                c => c.GetPRNTransactionsByRentGroupAsync(
                    It.Is<string>(rg => rg == expectedTransactionFilters.RentGroup),
                    It.Is<int>(fy => fy == expectedTransactionFilters.FinancialYear),
                    It.Is<int>(sp => sp == expectedTransactionFilters.StartWeekOrMonth),
                    It.Is<int>(ep => ep == expectedTransactionFilters.EndWeekOrMonth)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetPRNTransactionsGatewayMethodReturnsDbContextResultThatIsMappedToDomain()
        {
            // arrange
            var unmappedDbContextReturn = RandomGen.CreateMany<PRNTransactionEntity>();

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetPRNTransactionsByRentGroupAsync(
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<int>()
                    )
                )
                .ReturnsAsync(unmappedDbContextReturn);

            var transactionFilters = RandomGen.Create<GetPRNTransactionsDomain>();
            var expectedMappedGWReturn = unmappedDbContextReturn.ToDomain();

            // act
            var transactionsGWReturn = await _classUnderTest
                .GetPRNTransactions(transactionFilters)
                .ConfigureAwait(false);

            // assert
            transactionsGWReturn.Should().HaveSameCount(expectedMappedGWReturn);
            transactionsGWReturn.Should().BeEquivalentTo(expectedMappedGWReturn);
        }

        [Fact]
        public async Task GetPRNTransactionsGatewayMethodThrowsWhenDbContextThrows()
        {
            // arrange
            var expectedErrorMessage = "Database is taking a team lunch with its cluster mates.";
            var dbContextException = new ConnectionAbortedException(expectedErrorMessage);

            _mockHFSDatabaseContext
                .Setup(
                    g => g.GetPRNTransactionsByRentGroupAsync(
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<int>()
                    )
                )
                .ThrowsAsync(dbContextException);

            var irrelevantFilters = RandomGen.Create<GetPRNTransactionsDomain>();

            // act
            Func<Task> transactionGatewayCall = async () => await _classUnderTest
                .GetPRNTransactions(irrelevantFilters)
                .ConfigureAwait(false);

            // assert
            await transactionGatewayCall
                .Should()
                .ThrowAsync<ConnectionAbortedException>()
                .WithMessage(expectedErrorMessage)
                .ConfigureAwait(false);
        }
    }
}
