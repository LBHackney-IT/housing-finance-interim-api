using FluentAssertions;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using Xunit;
using I = HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.Tests.V1.Factories
{
    public class BatchReportFactoryTests
    {
        public BatchReportFactoryTests()
        {

        }

        #region Shared
        [Fact]
        public void BatchLogGetsCorrectlyMappedFromDomainToDatabase()
        {
            // arrange
            var batchReportDomain = RandomGen.Create<BatchReportDomain>();

            // act
            var batchReportInfrastructure = batchReportDomain.ToDatabase();

            // assert
            batchReportInfrastructure.Should().NotBeNull();

            batchReportInfrastructure.Id.Should().Be(batchReportDomain.Id);
            batchReportInfrastructure.ReportName.Should().Be(batchReportDomain.ReportName);
            batchReportInfrastructure.RentGroup.Should().Be(batchReportDomain.RentGroup);
            batchReportInfrastructure.Group.Should().Be(batchReportDomain.Group);
            batchReportInfrastructure.TransactionType.Should().Be(batchReportDomain.TransactionType);
            batchReportInfrastructure.ReportStartDate.Should().Be(batchReportDomain.ReportStartDate);
            batchReportInfrastructure.ReportEndDate.Should().Be(batchReportDomain.ReportEndDate);
            batchReportInfrastructure.ReportDate.Should().Be(batchReportDomain.ReportDate);
            batchReportInfrastructure.ReportYear.Should().Be(batchReportDomain.ReportYear);
            batchReportInfrastructure.Link.Should().Be(batchReportDomain.Link);
            batchReportInfrastructure.StartTime.Should().Be(batchReportDomain.StartTime);
            batchReportInfrastructure.EndTime.Should().Be(batchReportDomain.EndTime);
            batchReportInfrastructure.IsSuccess.Should().Be(batchReportDomain.IsSuccess);
        }

        [Fact]
        public void BatchLogGetsCorrectlyMappedFromDatabaseToDomain()
        {
            // arrange
            var batchReportInfrastructure = RandomGen.Create<I.BatchReport>();

            // act
            var batchReportDomain = batchReportInfrastructure.ToDomain();

            // assert
            batchReportDomain.Should().NotBeNull();

            batchReportDomain.Id.Should().Be(batchReportInfrastructure.Id);
            batchReportDomain.ReportName.Should().Be(batchReportInfrastructure.ReportName);
            batchReportDomain.RentGroup.Should().Be(batchReportInfrastructure.RentGroup);
            batchReportDomain.Group.Should().Be(batchReportInfrastructure.Group);
            batchReportDomain.TransactionType.Should().Be(batchReportInfrastructure.TransactionType);
            batchReportDomain.ReportStartDate.Should().Be(batchReportInfrastructure.ReportStartDate);
            batchReportDomain.ReportEndDate.Should().Be(batchReportInfrastructure.ReportEndDate);
            batchReportDomain.ReportDate.Should().Be(batchReportInfrastructure.ReportDate);
            batchReportDomain.ReportYear.Should().Be(batchReportInfrastructure.ReportYear);
            batchReportDomain.Link.Should().Be(batchReportInfrastructure.Link);
            batchReportDomain.StartTime.Should().Be(batchReportInfrastructure.StartTime);
            batchReportDomain.EndTime.Should().Be(batchReportInfrastructure.EndTime);
            batchReportDomain.IsSuccess.Should().Be(batchReportInfrastructure.IsSuccess);
        }

        [Fact]
        public void BatchLogGetsMappedToNullIfSourceIsNullBetweenDomainAndDatabase()
        {
            // arrange
            BatchReportDomain batchReportDomain = null;
            I.BatchReport batchReportInfrastructure = null;

            // act
            var resultBRInfrastructure = batchReportDomain.ToDatabase();
            var resultBRDomain = batchReportInfrastructure.ToDomain();

            // assert
            resultBRInfrastructure.Should().BeNull();
            resultBRDomain.Should().BeNull();
        }
        #endregion

        #region Account Balance
        [Fact]
        public void BatchReportAccountBalanceRequestGetsMappedToDomain()
        {
            // arrange
            var batchReportABRequest = RandomGen.Create<BatchReportAccountBalanceRequest>();

            // act
            var batchReportDomain = batchReportABRequest.ToDomain();

            // assert
            batchReportDomain.Should().NotBeNull();

            batchReportDomain.Id.Should().Be(default);
            batchReportDomain.ReportName.Should().Be(default);
            batchReportDomain.RentGroup.Should().Be(batchReportABRequest.RentGroup);
            batchReportDomain.Group.Should().Be(default);
            batchReportDomain.TransactionType.Should().Be(default);
            batchReportDomain.ReportStartDate.Should().Be(default);
            batchReportDomain.ReportEndDate.Should().Be(default);
            batchReportDomain.ReportDate.Should().Be(batchReportABRequest.ReportDate);
            batchReportDomain.ReportYear.Should().Be(default(int?));
            batchReportDomain.Link.Should().Be(default);
            batchReportDomain.StartTime.Should().Be(default);
            batchReportDomain.EndTime.Should().Be(default);
            batchReportDomain.IsSuccess.Should().Be(default);
        }

        [Fact]
        public void BatchReportAccountBalanceGetsMappedToNullIfSourceIsNullBetweenDomainAndPresentation()
        {
            // arrange
            BatchReportAccountBalanceRequest batchReportABRequest = null;
            BatchReportDomain batchReportDomain = null;

            // act
            var resultBRDomain = batchReportABRequest.ToDomain();
            var resultBRABResponse = batchReportDomain.ToReportAccountBalanceResponse();

            // assert
            resultBRDomain.Should().BeNull();
            resultBRABResponse.Should().BeNull();
        }

        [Fact]
        public void BatchReportDomainGetsMappedToBatchReportAccountBalanceResponse()
        {
            // arrange
            var batchReportDomain = RandomGen.Create<BatchReportDomain>();

            // act
            var batchReportABResponse = batchReportDomain.ToReportAccountBalanceResponse();

            // assert
            batchReportABResponse.Should().NotBeNull();

            batchReportABResponse.Id.Should().Be(batchReportDomain.Id);
            batchReportABResponse.RentGroup.Should().Be(batchReportDomain.RentGroup);
            batchReportABResponse.ReportDate.Should().Be(batchReportDomain.ReportDate.Value);
            batchReportABResponse.Link.Should().Be(batchReportDomain.Link);
            batchReportABResponse.StartTime.Should().Be(batchReportDomain.StartTime);
            batchReportABResponse.EndTime.Should().Be(batchReportDomain.EndTime);
            batchReportABResponse.IsSuccess.Should().Be(batchReportDomain.IsSuccess);
        }
        #endregion

        #region Charges
        [Fact]
        public void BatchReportChargesRequestGetsMappedToDomain()
        {
            // arrange
            var batchReportCRequest = RandomGen.Create<BatchReportChargesRequest>();

            // act
            var batchReportDomain = batchReportCRequest.ToDomain();

            // assert
            batchReportDomain.Should().NotBeNull();

            batchReportDomain.Id.Should().Be(default);
            batchReportDomain.ReportName.Should().Be(default);
            batchReportDomain.RentGroup.Should().Be(batchReportCRequest.RentGroup);
            batchReportDomain.Group.Should().Be(batchReportCRequest.Group);
            batchReportDomain.TransactionType.Should().Be(default);
            batchReportDomain.ReportStartDate.Should().Be(default);
            batchReportDomain.ReportEndDate.Should().Be(default);
            batchReportDomain.ReportDate.Should().Be(default);
            batchReportDomain.ReportYear.Should().Be(batchReportCRequest.Year);
            batchReportDomain.Link.Should().Be(default);
            batchReportDomain.StartTime.Should().Be(default);
            batchReportDomain.EndTime.Should().Be(default);
            batchReportDomain.IsSuccess.Should().Be(default);
        }

        [Fact]
        public void BatchReportChargesGetsMappedToNullIfSourceIsNullBetweenDomainAndPresentation()
        {
            // arrange
            BatchReportChargesRequest batchReportCRequest = null;
            BatchReportDomain batchReportDomain = null;

            // act
            var resultBRDomain = batchReportCRequest.ToDomain();
            var resultBRCResponse = batchReportDomain.ToReportChargesResponse();

            // assert
            resultBRDomain.Should().BeNull();
            resultBRCResponse.Should().BeNull();
        }

        [Fact]
        public void BatchReportDomainGetsMappedToBatchReportChargesResponse()
        {
            // arrange
            var batchReportDomain = RandomGen.Create<BatchReportDomain>();

            // act
            var batchReportCResponse = batchReportDomain.ToReportChargesResponse();

            // assert
            batchReportCResponse.Should().NotBeNull();

            batchReportCResponse.Id.Should().Be(batchReportDomain.Id);
            batchReportCResponse.Year.Should().Be(batchReportDomain.ReportYear);
            batchReportCResponse.RentGroup.Should().Be(batchReportDomain.RentGroup);
            batchReportCResponse.Group.Should().Be(batchReportDomain.Group);
            batchReportCResponse.Link.Should().Be(batchReportDomain.Link);
            batchReportCResponse.StartTime.Should().Be(batchReportDomain.StartTime);
            batchReportCResponse.EndTime.Should().Be(batchReportDomain.EndTime);
            batchReportCResponse.IsSuccess.Should().Be(batchReportDomain.IsSuccess);
        }
        #endregion

        #region Itemised Transactions
        [Fact]
        public void BatchReportItemisedTransactionRequestGetsMappedToDomain()
        {
            // arrange
            var batchReportITRequest = RandomGen.Create<BatchReportItemisedTransactionRequest>();

            // act
            var batchReportDomain = batchReportITRequest.ToDomain();

            // assert
            batchReportDomain.Should().NotBeNull();

            batchReportDomain.Id.Should().Be(default);
            batchReportDomain.ReportName.Should().Be(default);
            batchReportDomain.RentGroup.Should().Be(default);
            batchReportDomain.Group.Should().Be(default);
            batchReportDomain.TransactionType.Should().Be(batchReportITRequest.TransactionType);
            batchReportDomain.ReportStartDate.Should().Be(default);
            batchReportDomain.ReportEndDate.Should().Be(default);
            batchReportDomain.ReportDate.Should().Be(default);
            batchReportDomain.ReportYear.Should().Be(batchReportITRequest.Year);
            batchReportDomain.Link.Should().Be(default);
            batchReportDomain.StartTime.Should().Be(default);
            batchReportDomain.EndTime.Should().Be(default);
            batchReportDomain.IsSuccess.Should().Be(default);
        }

        [Fact]
        public void BatchReportItemisedTransactionGetsMappedToNullIfSourceIsNullBetweenDomainAndPresentation()
        {
            // arrange
            BatchReportItemisedTransactionRequest batchReportITRequest = null;
            BatchReportDomain batchReportDomain = null;

            // act
            var resultBRDomain = batchReportITRequest.ToDomain();
            var resultBRITResponse = batchReportDomain.ToReportItemisedTransactionResponse();

            // assert
            resultBRDomain.Should().BeNull();
            resultBRITResponse.Should().BeNull();
        }

        [Fact]
        public void BatchReportDomainGetsMappedToBatchReportItemisedTransactionResponse()
        {
            // arrange
            var batchReportDomain = RandomGen.Create<BatchReportDomain>();

            // act
            var batchReportITResponse = batchReportDomain.ToReportItemisedTransactionResponse();

            // assert
            batchReportITResponse.Should().NotBeNull();
        
            batchReportITResponse.Id.Should().Be(batchReportDomain.Id);
            batchReportITResponse.Year.Should().Be(batchReportDomain.ReportYear);
            batchReportITResponse.TransactionType.Should().Be(batchReportDomain.TransactionType);
            batchReportITResponse.Link.Should().Be(batchReportDomain.Link);
            batchReportITResponse.StartTime.Should().Be(batchReportDomain.StartTime);
            batchReportITResponse.EndTime.Should().Be(batchReportDomain.EndTime);
            batchReportITResponse.IsSuccess.Should().Be(batchReportDomain.IsSuccess);
        }
        #endregion

        #region Cash Suspense
        [Fact]
        public void BatchReportCashSuspenseRequestGetsMappedToDomain()
        {
            // arrange
            var batchReportCSRequest = RandomGen.Create<BatchReportCashSuspenseRequest>();

            // act
            var batchReportDomain = batchReportCSRequest.ToDomain();

            // assert
            batchReportDomain.Should().NotBeNull();

            batchReportDomain.Id.Should().Be(default);
            batchReportDomain.ReportName.Should().Be(default);
            batchReportDomain.RentGroup.Should().Be(default);
            batchReportDomain.Group.Should().Be(batchReportCSRequest.SuspenseAccountType);
            batchReportDomain.TransactionType.Should().Be(default);
            batchReportDomain.ReportStartDate.Should().Be(default);
            batchReportDomain.ReportEndDate.Should().Be(default);
            batchReportDomain.ReportDate.Should().Be(default);
            batchReportDomain.ReportYear.Should().Be(batchReportCSRequest.Year);
            batchReportDomain.Link.Should().Be(default);
            batchReportDomain.StartTime.Should().Be(default);
            batchReportDomain.EndTime.Should().Be(default);
            batchReportDomain.IsSuccess.Should().Be(default);
        }

        [Fact]
        public void BatchReportCashSuspenseGetsMappedToNullIfSourceIsNullBetweenDomainAndPresentation()
        {
            // arrange
            BatchReportCashSuspenseRequest batchReportCSRequest = null;
            BatchReportDomain batchReportDomain = null;

            // act
            var resultBRDomain = batchReportCSRequest.ToDomain();
            var resultBRCSResponse = batchReportDomain.ToReportCashSuspenseResponse();

            // assert
            resultBRDomain.Should().BeNull();
            resultBRCSResponse.Should().BeNull();
        }

        [Fact]
        public void BatchReportDomainGetsMappedToBatchReportCashSuspenseResponse()
        {
            // arrange
            var batchReportDomain = RandomGen.Create<BatchReportDomain>();

            // act
            var batchReportCSResponse = batchReportDomain.ToReportCashSuspenseResponse();

            // assert
            batchReportCSResponse.Should().NotBeNull();

            batchReportCSResponse.Id.Should().Be(batchReportDomain.Id);
            batchReportCSResponse.Year.Should().Be(batchReportDomain.ReportYear);
            batchReportCSResponse.SuspenseAccountType.Should().Be(batchReportDomain.Group);
            batchReportCSResponse.Link.Should().Be(batchReportDomain.Link);
            batchReportCSResponse.StartTime.Should().Be(batchReportDomain.StartTime);
            batchReportCSResponse.EndTime.Should().Be(batchReportDomain.EndTime);
            batchReportCSResponse.IsSuccess.Should().Be(batchReportDomain.IsSuccess);
        }
        #endregion

        #region Cash Import
        [Fact]
        public void BatchReportCashImportRequestGetsMappedToDomain()
        {
            // arrange
            var batchReportCIRequest = RandomGen.Create<BatchReportCashImportRequest>();

            // act
            var batchReportDomain = batchReportCIRequest.ToDomain();

            // assert
            batchReportDomain.Should().NotBeNull();

            batchReportDomain.Id.Should().Be(default);
            batchReportDomain.ReportName.Should().Be(default);
            batchReportDomain.RentGroup.Should().Be(default);
            batchReportDomain.Group.Should().Be(default);
            batchReportDomain.TransactionType.Should().Be(default);
            batchReportDomain.ReportStartDate.Should().Be(batchReportCIRequest.StartDate);
            batchReportDomain.ReportEndDate.Should().Be(batchReportCIRequest.EndDate);
            batchReportDomain.ReportDate.Should().Be(default);
            batchReportDomain.ReportYear.Should().Be(default(int?));
            batchReportDomain.Link.Should().Be(default);
            batchReportDomain.StartTime.Should().Be(default);
            batchReportDomain.EndTime.Should().Be(default);
            batchReportDomain.IsSuccess.Should().Be(default);
        }

        [Fact]
        public void BatchReportCashImportGetsMappedToNullIfSourceIsNullBetweenDomainAndPresentation()
        {
            // arrange
            BatchReportCashImportRequest batchReportCIRequest = null;
            BatchReportDomain batchReportDomain = null;

            // act
            var resultBRDomain = batchReportCIRequest.ToDomain();
            var resultBRCIResponse = batchReportDomain.ToReportCashImportResponse();

            // assert
            resultBRDomain.Should().BeNull();
            resultBRCIResponse.Should().BeNull();
        }

        [Fact]
        public void BatchReportDomainGetsMappedToBatchReportCashImportResponse()
        {
            // arrange
            var batchReportDomain = RandomGen.Create<BatchReportDomain>();

            // act
            var batchReportCIResponse = batchReportDomain.ToReportCashImportResponse();

            // assert
            batchReportCIResponse.Should().NotBeNull();

            batchReportCIResponse.Id.Should().Be(batchReportDomain.Id);
            batchReportCIResponse.StartDate.Should().Be(batchReportDomain.ReportStartDate.Value);
            batchReportCIResponse.EndDate.Should().Be(batchReportDomain.ReportEndDate.Value);
            batchReportCIResponse.Link.Should().Be(batchReportDomain.Link);
            batchReportCIResponse.StartTime.Should().Be(batchReportDomain.StartTime);
            batchReportCIResponse.EndTime.Should().Be(batchReportDomain.EndTime);
            batchReportCIResponse.IsSuccess.Should().Be(batchReportDomain.IsSuccess);
        }
        #endregion

        #region Housing Benefit Academy
        [Fact]
        public void BatchReportHousingBenefitAcademyRequestGetsMappedToDomain()
        {
            // arrange
            var batchReportHBARequest = RandomGen.Create<BatchReportHousingBenefitAcademyRequest>();

            // act
            var batchReportDomain = batchReportHBARequest.ToDomain();

            // assert
            batchReportDomain.Should().NotBeNull();

            batchReportDomain.Id.Should().Be(default);
            batchReportDomain.ReportName.Should().Be(default);
            batchReportDomain.RentGroup.Should().Be(default);
            batchReportDomain.Group.Should().Be(default);
            batchReportDomain.TransactionType.Should().Be(default);
            batchReportDomain.ReportStartDate.Should().Be(default);
            batchReportDomain.ReportEndDate.Should().Be(default);
            batchReportDomain.ReportDate.Should().Be(default);
            batchReportDomain.ReportYear.Should().Be(batchReportHBARequest.Year);
            batchReportDomain.Link.Should().Be(default);
            batchReportDomain.StartTime.Should().Be(default);
            batchReportDomain.EndTime.Should().Be(default);
            batchReportDomain.IsSuccess.Should().Be(default);
        }

        [Fact]
        public void BatchReportHousingBenefitAcademyGetsMappedToNullIfSourceIsNullBetweenDomainAndPresentation()
        {
            // arrange
            BatchReportHousingBenefitAcademyRequest batchReportHBARequest = null;
            BatchReportDomain batchReportDomain = null;

            // act
            var resultBRDomain = batchReportHBARequest.ToDomain();
            var resultBRHBAResponse = batchReportDomain.ToReportHousingBenefitAcademyResponse();

            // assert
            resultBRDomain.Should().BeNull();
            resultBRHBAResponse.Should().BeNull();
        }

        [Fact]
        public void BatchReportDomainGetsMappedToBatchReportHousingBenefitAcademyResponse()
        {
            // arrange
            var batchReportDomain = RandomGen.Create<BatchReportDomain>();

            // act
            var batchReportHBAResponse = batchReportDomain.ToReportHousingBenefitAcademyResponse();

            // assert
            batchReportHBAResponse.Should().NotBeNull();

            batchReportHBAResponse.Id.Should().Be(batchReportDomain.Id);
            batchReportHBAResponse.Year.Should().Be(batchReportDomain.ReportYear);
            batchReportHBAResponse.Link.Should().Be(batchReportDomain.Link);
            batchReportHBAResponse.StartTime.Should().Be(batchReportDomain.StartTime);
            batchReportHBAResponse.EndTime.Should().Be(batchReportDomain.EndTime);
            batchReportHBAResponse.IsSuccess.Should().Be(batchReportDomain.IsSuccess);
        }
        #endregion
    }
}
