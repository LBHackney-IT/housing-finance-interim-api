using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Controllers;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Controllers
{
    public class ReportControllerTests
    {
        private readonly Mock<IBatchReportGateway> _batchReportGatewayMock;
        private readonly ReportController _classUnderTest;

        public ReportControllerTests()
        {
            _batchReportGatewayMock = new Mock<IBatchReportGateway>();
            _classUnderTest = new ReportController(_batchReportGatewayMock.Object);
        }

        #region Account Balance
        [Fact]
        public async Task CreateReportAccountBalanceReturnsSuccessResponseWithDataMatchingGWResponse()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportAccountBalanceRequest>();
            var batchReportAccountBalanceGWResult = RandomGen.Create<BatchReportDomain>();
            var batchReportAccountBalanceResponse = batchReportAccountBalanceGWResult.ToReportAccountBalanceResponse();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportAccountBalanceGWResult);

            // Act
            var response = await _classUnderTest
                .CreateReportAccountBalance(request)
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);

            var responseObject = createdResult.Value as BatchReportAccountBalanceResponse;
            responseObject.Should().BeEquivalentTo(batchReportAccountBalanceResponse);
        }

        [Fact]
        public async Task CreateReportAccountBalanceCallsBatchReportGWCreateMethodWithCorrectParameters()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportAccountBalanceRequest>();
            var batchReportAccountBalanceGWResult = RandomGen.Create<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportAccountBalanceGWResult);

            // Act
            await _classUnderTest
                .CreateReportAccountBalance(request)
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.CreateAsync(It.Is<BatchReportDomain>(brd =>
                    brd.RentGroup == request.RentGroup &&
                    brd.ReportDate == request.ReportDate &&
                    brd.ReportName == "ReportAccountBalanceByDate"
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateReportAccountBalanceThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportAccountBalanceRequest>();
            var message = "Method has a nullphobia.";

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ThrowsAsync(new ArgumentException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .CreateReportAccountBalance(request)
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<ArgumentException>().WithMessage(message);
        }

        [Fact]
        public async Task ListReportAccountBalanceCallsBatchReportGWListMethodWithCorrectParameters()
        {
            // Arrange
            var batchReportAccountBalanceGWResult = new List<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportAccountBalanceGWResult);

            // Act
            await _classUnderTest
                .ListReportAccountBalance()
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.ListAsync(It.Is<string>(reportName => reportName == "ReportAccountBalanceByDate")),
                Times.Once
            );
        }

        [Fact]
        public async Task ListReportAccountBalanceReturnsListOfBatchReportAccountBalanceResponseItemsMatchingGWResponseWhenExecutionEndInSuccess()
        {
            // Arrange
            var batchReportAccountBalanceGWResult = RandomGen.CreateMany<BatchReportDomain>();
            var batchReportAccountBalanceResponse = batchReportAccountBalanceGWResult.ToReportAccountBalanceResponse();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportAccountBalanceGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportAccountBalance()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as OkObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(200);

            var responseObject = createdResult.Value as List<BatchReportAccountBalanceResponse>;
            responseObject.Should().HaveSameCount(batchReportAccountBalanceGWResult);
            responseObject.Should().BeEquivalentTo(batchReportAccountBalanceResponse);
        }

        [Fact]
        public async Task ListReportAccountBalanceReturnsEmptyListResponseWhenGatewayFindsNoItems()
        {
            // Arrange
            var batchReportAccountBalanceGWResult = new List<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportAccountBalanceGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportAccountBalance()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as OkObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(200);

            var responseObject = createdResult.Value as List<BatchReportAccountBalanceResponse>;
            responseObject.Should().HaveCount(0);
        }

        [Fact]
        public async Task ListReportAccountBalanceReturnsA404NotFoundResponseWhenGatewayIsUnableToFindTheCollection()
        {
            // Arrange
            IList<BatchReportDomain> batchReportAccountBalanceGWResult = null;

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportAccountBalanceGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportAccountBalance()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as NotFoundResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ListReportAccountBalanceThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var message = "Undergoing database maintenance.";

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ThrowsAsync(new ConnectionAbortedException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .ListReportAccountBalance()
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<ConnectionAbortedException>().WithMessage(message);
        }
        #endregion

        #region Charges
        [Fact]
        public async Task CreateReportChargesReturnsSuccessResponseWithDataMatchingGWResponse()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportChargesRequest>();
            var batchReportChargesGWResult = RandomGen.Create<BatchReportDomain>();
            var batchReportChargesResponse = batchReportChargesGWResult.ToReportChargesResponse();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportChargesGWResult);

            // Act
            var response = await _classUnderTest
                .CreateReportCharges(request)
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);

            var responseObject = createdResult.Value as BatchReportChargesResponse;
            responseObject.Should().BeEquivalentTo(batchReportChargesResponse);
        }

        [Fact]
        public async Task CreateReportChargesCallsBatchReportGWCreateMethodWithCorrectParameters()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportChargesRequest>();
            var batchReportChargesGWResult = RandomGen.Create<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportChargesGWResult);

            // Act
            await _classUnderTest
                .CreateReportCharges(request)
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.CreateAsync(It.Is<BatchReportDomain>(brd =>
                    brd.ReportYear == request.Year &&
                    brd.RentGroup == request.RentGroup &&
                    brd.Group == request.Group &&
                    brd.ReportName == "ReportCharges"
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateReportChargesThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportChargesRequest>();
            var message = "The gateway cannot wait any longer, it has other commitments to attend to!";

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ThrowsAsync(new TimeoutException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .CreateReportCharges(request)
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<TimeoutException>().WithMessage(message);
        }

        [Fact]
        public async Task ListReportChargesCallsBatchReportGWListMethodWithCorrectParameters()
        {
            // Arrange
            var batchReportChargesGWResult = new List<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportChargesGWResult);

            // Act
            await _classUnderTest
                .ListReportCharges()
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.ListAsync(It.Is<string>(reportName => reportName == "ReportCharges")),
                Times.Once
            );
        }

        [Fact]
        public async Task ListReportChargesReturnsListOfBatchReportChargesResponseItemsMatchingGWResponseWhenExecutionEndInSuccess()
        {
            // Arrange
            var batchReportChargesGWResult = RandomGen.CreateMany<BatchReportDomain>();
            var batchReportChargesResponse = batchReportChargesGWResult.ToReportChargesResponse();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportChargesGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportCharges()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as OkObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(200);

            var responseObject = createdResult.Value as List<BatchReportChargesResponse>;
            responseObject.Should().HaveSameCount(batchReportChargesGWResult);
            responseObject.Should().BeEquivalentTo(batchReportChargesResponse);
        }

        [Fact]
        public async Task ListReportChargesReturnsEmptyListResponseWhenGatewayFindsNoItems()
        {
            // Arrange
            var batchReportChargesGWResult = new List<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportChargesGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportCharges()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as OkObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(200);

            var responseObject = createdResult.Value as List<BatchReportChargesResponse>;
            responseObject.Should().HaveCount(0);
        }

        [Fact]
        public async Task ListReportChargesReturnsA404NotFoundResponseWhenGatewayIsUnableToFindTheCollection()
        {
            // Arrange
            IList<BatchReportDomain> batchReportChargesGWResult = null;

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportChargesGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportCharges()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as NotFoundResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ListReportChargesThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var message = "The database is going on the lunch break.";

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ThrowsAsync(new ConnectionAbortedException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .ListReportCharges()
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<ConnectionAbortedException>().WithMessage(message);
        }
        #endregion

        #region Operating Balances by Rent Account
        [Fact]
        public async Task CreateReportOperatingBalancesByRentAccountReturnsSuccessResponseWithDataMatchingGWResponse()
        {
            // Arrange
            var irrelevant = RandomGen.Create<BatchReportOperatingBalancesByRentAccountRequest>();
            var batchReportOBRAGWResult = RandomGen.Create<BatchReportDomain>();
            var batchReportOBRAResponse = batchReportOBRAGWResult.ToReportOperatingBalancesByRentAccountResponse();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportOBRAGWResult);

            // Act
            var response = await _classUnderTest
                .CreateReportOperatingBalancesByRentAccount(irrelevant)
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);

            var responseObject = createdResult.Value as BatchReportOperatingBalancesByRentAccountResponse;
            responseObject.Should().BeEquivalentTo(batchReportOBRAResponse);
        }

        [Fact]
        public async Task CreateReportOperatingBalancesByRentAccountCallsBatchReportGWCreateMethodWithCorrectParameters()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportOperatingBalancesByRentAccountRequest>();
            var irrelevant = RandomGen.Create<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(irrelevant);

            // Act
            await _classUnderTest
                .CreateReportOperatingBalancesByRentAccount(request)
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.CreateAsync(It.Is<BatchReportDomain>(brd =>
                    brd.RentGroup == request.RentGroup &&
                    brd.ReportYear == request.FinancialYear &&
                    brd.ReportStartWeekOrMonth == request.StartWeekOrMonth &&
                    brd.ReportEndWeekOrMonth == request.EndWeekOrMonth &&
                    brd.ReportName == "ReportOperatingBalancesByRentAccount"
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateReportOperatingBalancesByRentAccountThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportOperatingBalancesByRentAccountRequest>();
            var message = "15 minute lambda timeout reached!";

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ThrowsAsync(new TimeoutException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .CreateReportOperatingBalancesByRentAccount(request)
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<TimeoutException>().WithMessage(message);
        }

        [Fact]
        public async Task CreateReportOperatingBalancesByRentAccountReturns400BadRequestWhenModelStateErrorIsEncountered()
        {
            // Arrange
            var request = RandomGen
                .Build<BatchReportOperatingBalancesByRentAccountRequest>()
                .With(r => r.FinancialYear, default(int))
                .CreateCustom();

            _classUnderTest.ModelState.AddModelError("FinancialYear", "The FinancialYear field is required.");

            // Act
            var response = await _classUnderTest
                .CreateReportOperatingBalancesByRentAccount(request)
                .ConfigureAwait(false);

            // Assert
            var statusCodeResult = response as IStatusCodeActionResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(400);
        }
        #endregion

        #region Itemised Transactions
        [Fact]
        public async Task CreateReportItemisedTransactionReturnsSuccessResponseWithDataMatchingGWResponse()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportItemisedTransactionRequest>();
            var batchReportItemisedTransactionGWResult = RandomGen.Create<BatchReportDomain>();
            var batchReportItemisedTransactionResponse = batchReportItemisedTransactionGWResult.ToReportItemisedTransactionResponse();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportItemisedTransactionGWResult);

            // Act
            var response = await _classUnderTest
                .CreateReportItemisedTransaction(request)
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);

            var responseObject = createdResult.Value as BatchReportItemisedTransactionResponse;
            responseObject.Should().BeEquivalentTo(batchReportItemisedTransactionResponse);
        }

        [Fact]
        public async Task CreateReportItemisedTransactionCallsBatchReportGWCreateMethodWithCorrectParameters()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportItemisedTransactionRequest>();
            var batchReportItemisedTransactionGWResult = RandomGen.Create<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportItemisedTransactionGWResult);

            // Act
            await _classUnderTest
                .CreateReportItemisedTransaction(request)
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.CreateAsync(It.Is<BatchReportDomain>(brd =>
                    brd.ReportYear == request.Year &&
                    brd.TransactionType == request.TransactionType &&
                    brd.ReportName == "ReportItemisedTransactions"
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateReportItemisedTransactionThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportItemisedTransactionRequest>();
            var message = "15 minute lambda timeout reached!";

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ThrowsAsync(new TimeoutException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .CreateReportItemisedTransaction(request)
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<TimeoutException>().WithMessage(message);
        }

        [Fact]
        public async Task CreateReportItemisedTransactionReturns400BadRequestWhenFinancialYearIsNotSpecified()
        {
            // Arrange
            var request = RandomGen
                .Build<BatchReportItemisedTransactionRequest>()
                .With(r => r.Year, default(int))
                .CreateCustom();

            _classUnderTest.ModelState.AddModelError("Year", "The Year field is required.");

            // Act
            var response = await _classUnderTest
                .CreateReportItemisedTransaction(request)
                .ConfigureAwait(false);

            // Assert
            var statusCodeResult = response as IStatusCodeActionResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task CreateReportItemisedTransactionReturns400BadRequestWhenFinancialTransactionTypeIsNotSpecified()
        {
            // Arrange
            var request = RandomGen
                .Build<BatchReportItemisedTransactionRequest>()
                .With(r => r.TransactionType, default(string))
                .CreateCustom();

            _classUnderTest.ModelState.AddModelError("TransactionType", "The TransactionType field is required.");

            // Act
            var response = await _classUnderTest
                .CreateReportItemisedTransaction(request)
                .ConfigureAwait(false);

            // Assert
            var statusCodeResult = response as IStatusCodeActionResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task ListReportItemisedTransactionsCallsBatchReportGWListMethodWithCorrectParameters()
        {
            // Arrange
            var batchReportItemisedTransactionsGWResult = new List<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportItemisedTransactionsGWResult);

            // Act
            await _classUnderTest
                .ListReportItemisedTransactions()
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.ListAsync(It.Is<string>(reportName => reportName == "ReportItemisedTransactions")),
                Times.Once
            );
        }

        [Fact]
        public async Task ListReportItemisedTransactionsReturnsListOfBatchReportItemisedTransactionResponseItemsMatchingGWResponseWhenExecutionEndInSuccess()
        {
            // Arrange
            var batchReportItemisedTransactionsGWResult = RandomGen.CreateMany<BatchReportDomain>();
            var batchReportItemisedTransactionResponse = batchReportItemisedTransactionsGWResult.ToReportItemisedTransactionsResponse();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportItemisedTransactionsGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportItemisedTransactions()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as OkObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(200);

            var responseObject = createdResult.Value as List<BatchReportItemisedTransactionResponse>;
            responseObject.Should().HaveSameCount(batchReportItemisedTransactionsGWResult);
            responseObject.Should().BeEquivalentTo(batchReportItemisedTransactionResponse);
        }

        [Fact]
        public async Task ListReportItemisedTransactionsReturnsEmptyListResponseWhenGatewayFindsNoItems()
        {
            // Arrange
            var batchReportItemisedTransactionsGWResult = new List<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportItemisedTransactionsGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportItemisedTransactions()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as OkObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(200);

            var responseObject = createdResult.Value as List<BatchReportItemisedTransactionResponse>;
            responseObject.Should().HaveCount(0);
        }

        [Fact]
        public async Task ListReportItemisedTransactionsReturnsA404NotFoundResponseWhenGatewayIsUnableToFindTheCollection()
        {
            // Arrange
            IList<BatchReportDomain> batchReportItemisedTransactionsGWResult = null;

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportItemisedTransactionsGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportItemisedTransactions()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as NotFoundResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ListReportItemisedTransactionsThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var message = "Database credentials are not valid.";

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ThrowsAsync(new ConnectionAbortedException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .ListReportItemisedTransactions()
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<ConnectionAbortedException>().WithMessage(message);
        }
        #endregion

        #region Cash Suspense
        [Fact]
        public async Task CreateReportCashSuspenseReturnsSuccessResponseWithDataMatchingGWResponse()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportCashSuspenseRequest>();
            var batchReportCashSuspenseGWResult = RandomGen.Create<BatchReportDomain>();
            var batchReportCashSuspenseResponse = batchReportCashSuspenseGWResult.ToReportCashSuspenseResponse();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportCashSuspenseGWResult);

            // Act
            var response = await _classUnderTest
                .CreateReportCashSuspense(request)
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);

            var responseObject = createdResult.Value as BatchReportCashSuspenseResponse;
            responseObject.Should().BeEquivalentTo(batchReportCashSuspenseResponse);
        }

        [Fact]
        public async Task CreateReportCashSuspenseCallsBatchReportGWCreateMethodWithCorrectParameters()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportCashSuspenseRequest>();
            var batchReportCashSuspenseGWResult = RandomGen.Create<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportCashSuspenseGWResult);

            // Act
            await _classUnderTest
                .CreateReportCashSuspense(request)
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.CreateAsync(It.Is<BatchReportDomain>(brd =>
                    brd.ReportYear == request.Year &&
                    brd.Group == request.SuspenseAccountType &&
                    brd.ReportName == "ReportCashSuspense"
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateReportCashSuspenseThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportCashSuspenseRequest>();
            var message = "Connection 2 hour limit has ended.";

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ThrowsAsync(new TimeoutException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .CreateReportCashSuspense(request)
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<TimeoutException>().WithMessage(message);
        }

        [Fact]
        public async Task ListReportCashSuspenseCallsBatchReportGWListMethodWithCorrectParameters()
        {
            // Arrange
            var batchReportCashSuspenseGWResult = new List<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportCashSuspenseGWResult);

            // Act
            await _classUnderTest
                .ListReportCashSuspense()
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.ListAsync(It.Is<string>(reportName => reportName == "ReportCashSuspense")),
                Times.Once
            );
        }

        [Fact]
        public async Task ListReportCashSuspenseReturnsListOfBatchReportCashSuspenseResponseItemsMatchingGWResponseWhenExecutionEndInSuccess()
        {
            // Arrange
            var batchReportCashSuspenseGWResult = RandomGen.CreateMany<BatchReportDomain>();
            var batchReportCashSuspenseResponse = batchReportCashSuspenseGWResult.ToReportCashSuspenseResponse();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportCashSuspenseGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportCashSuspense()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as OkObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(200);

            var responseObject = createdResult.Value as List<BatchReportCashSuspenseResponse>;
            responseObject.Should().HaveSameCount(batchReportCashSuspenseGWResult);
            responseObject.Should().BeEquivalentTo(batchReportCashSuspenseResponse);
        }

        [Fact]
        public async Task ListReportCashSuspenseReturnsEmptyListResponseWhenGatewayFindsNoItems()
        {
            // Arrange
            var batchReportCashSuspenseGWResult = new List<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportCashSuspenseGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportCashSuspense()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as OkObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(200);

            var responseObject = createdResult.Value as List<BatchReportCashSuspenseResponse>;
            responseObject.Should().HaveCount(0);
        }

        [Fact]
        public async Task ListReportCashSuspenseReturnsA404NotFoundResponseWhenGatewayIsUnableToFindTheCollection()
        {
            // Arrange
            IList<BatchReportDomain> batchReportCashSuspenseGWResult = null;

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportCashSuspenseGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportCashSuspense()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as NotFoundResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ListReportCashSuspenseThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var message = "Database server is unreachable.";

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ThrowsAsync(new ConnectionAbortedException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .ListReportCashSuspense()
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<ConnectionAbortedException>().WithMessage(message);
        }
        #endregion

        #region Cash Import
        [Fact]
        public async Task CreateReportCashImportReturnsSuccessResponseWithDataMatchingGWResponse()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportCashImportRequest>();
            var batchReportCashImportGWResult = RandomGen.Create<BatchReportDomain>();
            var batchReportCashImportResponse = batchReportCashImportGWResult.ToReportCashImportResponse();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportCashImportGWResult);

            // Act
            var response = await _classUnderTest
                .CreateReportCashImport(request)
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);

            var responseObject = createdResult.Value as BatchReportCashImportResponse;
            responseObject.Should().BeEquivalentTo(batchReportCashImportResponse);
        }

        [Fact]
        public async Task CreateReportCashImportCallsBatchReportGWCreateMethodWithCorrectParameters()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportCashImportRequest>();
            var batchReportCashImportGWResult = RandomGen.Create<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportCashImportGWResult);

            // Act
            await _classUnderTest
                .CreateReportCashImport(request)
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.CreateAsync(It.Is<BatchReportDomain>(brd =>
                    brd.ReportStartDate == request.StartDate &&
                    brd.ReportEndDate == request.EndDate &&
                    brd.ReportName == "ReportCashImport"
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateReportCashImportThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportCashImportRequest>();
            var message = "Machine has ran out of available memory.";

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ThrowsAsync(new OutOfMemoryException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .CreateReportCashImport(request)
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<OutOfMemoryException>().WithMessage(message);
        }

        [Fact]
        public async Task ListReportCashImportCallsBatchReportGWListMethodWithCorrectParameters()
        {
            // Arrange
            var batchReportCashImportGWResult = new List<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportCashImportGWResult);

            // Act
            await _classUnderTest
                .ListReportCashImport()
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.ListAsync(It.Is<string>(reportName => reportName == "ReportCashImport")),
                Times.Once
            );
        }

        [Fact]
        public async Task ListReportCashImportReturnsListOfBatchReportCashImportResponseItemsMatchingGWResponseWhenExecutionEndInSuccess()
        {
            // Arrange
            var batchReportCashImportGWResult = RandomGen.CreateMany<BatchReportDomain>();
            var batchReportCashImportResponse = batchReportCashImportGWResult.ToReportCashImportResponse();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportCashImportGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportCashImport()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as OkObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(200);

            var responseObject = createdResult.Value as List<BatchReportCashImportResponse>;
            responseObject.Should().HaveSameCount(batchReportCashImportGWResult);
            responseObject.Should().BeEquivalentTo(batchReportCashImportResponse);
        }

        [Fact]
        public async Task ListReportCashImportReturnsEmptyListResponseWhenGatewayFindsNoItems()
        {
            // Arrange
            var batchReportCashImportGWResult = new List<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportCashImportGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportCashImport()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as OkObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(200);

            var responseObject = createdResult.Value as List<BatchReportCashImportResponse>;
            responseObject.Should().HaveCount(0);
        }

        [Fact]
        public async Task ListReportCashImportReturnsA404NotFoundResponseWhenGatewayIsUnableToFindTheCollection()
        {
            // Arrange
            IList<BatchReportDomain> batchReportCashImportGWResult = null;

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportCashImportGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportCashImport()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as NotFoundResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ListReportCashImportThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var message = "Unexpected parameter value.";

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .ListReportCashImport()
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<ArgumentException>().WithMessage(message);
        }
        #endregion

        #region Housing Benefit Academy
        [Fact]
        public async Task CreateReportHousingBenefitAcademyReturnsSuccessResponseWithDataMatchingGWResponse()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportHousingBenefitAcademyRequest>();
            var batchReportHousingBenefitAcademyGWResult = RandomGen.Create<BatchReportDomain>();
            var batchReportHousingBenefitAcademyResponse = batchReportHousingBenefitAcademyGWResult.ToReportHousingBenefitAcademyResponse();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportHousingBenefitAcademyGWResult);

            // Act
            var response = await _classUnderTest
                .CreateReportHousingBenefitAcademy(request)
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);

            var responseObject = createdResult.Value as BatchReportHousingBenefitAcademyResponse;
            responseObject.Should().BeEquivalentTo(batchReportHousingBenefitAcademyResponse);
        }

        [Fact]
        public async Task CreateReportHousingBenefitAcademyCallsBatchReportGWCreateMethodWithCorrectParameters()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportHousingBenefitAcademyRequest>();
            var batchReportHousingBenefitAcademyGWResult = RandomGen.Create<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ReturnsAsync(batchReportHousingBenefitAcademyGWResult);

            // Act
            await _classUnderTest
                .CreateReportHousingBenefitAcademy(request)
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.CreateAsync(It.Is<BatchReportDomain>(brd =>
                    brd.ReportYear == request.Year &&
                    brd.ReportName == "ReportHousingBenefitAcademy"
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateReportHousingBenefitAcademyThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var request = RandomGen.Create<BatchReportHousingBenefitAcademyRequest>();
            var message = "Object is not set set to an instance of an...";

            _batchReportGatewayMock
                .Setup(g => g.CreateAsync(It.IsAny<BatchReportDomain>()))
                .ThrowsAsync(new NullReferenceException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .CreateReportHousingBenefitAcademy(request)
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<NullReferenceException>().WithMessage(message);
        }

        [Fact]
        public async Task ListReportHousingBenefitAcademyCallsBatchReportGWListMethodWithCorrectParameters()
        {
            // Arrange
            var batchReportHousingBenefitAcademyGWResult = new List<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportHousingBenefitAcademyGWResult);

            // Act
            await _classUnderTest
                .ListReportHousingBenefitAcademy()
                .ConfigureAwait(false);

            // Assert
            _batchReportGatewayMock.Verify(
                g => g.ListAsync(It.Is<string>(reportName => reportName == "ReportHousingBenefitAcademy")),
                Times.Once
            );
        }

        [Fact]
        public async Task ListReportHousingBenefitAcademyReturnsListOfBatchReportHousingBenefitAcademyResponseItemsMatchingGWResponseWhenExecutionEndInSuccess()
        {
            // Arrange
            var batchReportHousingBenefitAcademyGWResult = RandomGen.CreateMany<BatchReportDomain>();
            var batchReportHousingBenefitAcademyResponse = batchReportHousingBenefitAcademyGWResult.ToReportHousingBenefitAcademyResponse();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportHousingBenefitAcademyGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportHousingBenefitAcademy()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as OkObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(200);

            var responseObject = createdResult.Value as List<BatchReportHousingBenefitAcademyResponse>;
            responseObject.Should().HaveSameCount(batchReportHousingBenefitAcademyGWResult);
            responseObject.Should().BeEquivalentTo(batchReportHousingBenefitAcademyResponse);
        }

        [Fact]
        public async Task ListReportHousingBenefitAcademyReturnsEmptyListResponseWhenGatewayFindsNoItems()
        {
            // Arrange
            var batchReportHousingBenefitAcademyGWResult = new List<BatchReportDomain>();

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportHousingBenefitAcademyGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportHousingBenefitAcademy()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as OkObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(200);

            var responseObject = createdResult.Value as List<BatchReportHousingBenefitAcademyResponse>;
            responseObject.Should().HaveCount(0);
        }

        [Fact]
        public async Task ListReportHousingBenefitAcademyReturnsA404NotFoundResponseWhenGatewayIsUnableToFindTheCollection()
        {
            // Arrange
            IList<BatchReportDomain> batchReportHousingBenefitAcademyGWResult = null;

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ReturnsAsync(batchReportHousingBenefitAcademyGWResult);

            // Act
            var response = await _classUnderTest
                .ListReportHousingBenefitAcademy()
                .ConfigureAwait(false);

            // Assert
            var createdResult = response as NotFoundResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ListReportHousingBenefitAcademyThrowsWhenAnExceptionIsRaisedDuringItsExecutionFlow()
        {
            // Arrange
            var message = "Entity could not be mapped because of an unexpected data type.";

            _batchReportGatewayMock
                .Setup(g => g.ListAsync(It.IsAny<string>()))
                .ThrowsAsync(new DataException(message));

            // Act
            Func<Task> endpointCall = async () => await _classUnderTest
                .ListReportHousingBenefitAcademy()
                .ConfigureAwait(false);

            // Assert
            await endpointCall.Should().ThrowAsync<DataException>().WithMessage(message);
        }
        #endregion
    }
}
