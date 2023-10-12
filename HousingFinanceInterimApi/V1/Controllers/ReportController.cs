using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Factories;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Boundary.Response;
using Hackney.Core.Authorization;

namespace HousingFinanceInterimApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/report")]
    [ApiVersion("1.0")]
    public class ReportController : BaseController
    {
        private readonly IBatchReportGateway _batchReportGateway;

        private const string ReportAccountBalanceByDateLabel = "ReportAccountBalanceByDate";
        private const string ReportChargesLabel = "ReportCharges";
        private const string ReportItemisedTransactionsLabel = "ReportItemisedTransactions";
        private const string ReportCashSuspenseLabel = "ReportCashSuspense";
        private const string ReportCashImportLabel = "ReportCashImport";
        private const string ReportHousingBenefitAcademyLabel = "ReportHousingBenefitAcademy";

        public ReportController(
            IBatchReportGateway batchReportGateway)
        {
            _batchReportGateway = batchReportGateway;
        }

        [ProducesResponseType(typeof(List<BatchReportChargesResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("charges")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> CreateReportCharges([FromBody] BatchReportChargesRequest request)
        {
            var batchReport = request.ToDomain();
            batchReport.ReportName = ReportChargesLabel;

            var batchReportCharges = await _batchReportGateway
                .CreateAsync(batchReport)
                .ConfigureAwait(false);

            return Created("Report request created",
                           batchReportCharges.ToReportChargesResponse());
        }

        [ProducesResponseType(typeof(List<BatchReportChargesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("charges")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> ListReportCharges()
        {
            var batchReportCharges = await _batchReportGateway
                .ListAsync(ReportChargesLabel).ConfigureAwait(false);

            if (batchReportCharges == null)
                return NotFound();
            return Ok(batchReportCharges.ToReportChargesResponse());
        }

        # region Itemised Transactions
        [ProducesResponseType(typeof(BatchReportItemisedTransactionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("itemised-transactions")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> CreateReportItemisedTransaction([FromBody] BatchReportItemisedTransactionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var batchReport = request.ToDomain();
            batchReport.ReportName = ReportItemisedTransactionsLabel;

            var batchReportItemisedTransaction = await _batchReportGateway
                .CreateAsync(batchReport)
                .ConfigureAwait(false);

            return Created(
                    "Report request created",
                    batchReportItemisedTransaction.ToReportItemisedTransactionResponse()
                );
        }

        [ProducesResponseType(typeof(List<BatchReportChargesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("itemised-transactions")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> ListReportItemisedTransactions()
        {
            var batchReportItemisedTransactions = await _batchReportGateway
                .ListAsync(ReportItemisedTransactionsLabel)
                .ConfigureAwait(false);

            if (batchReportItemisedTransactions is null)
                return NotFound();

            return Ok(batchReportItemisedTransactions.ToReportItemisedTransactionsResponse());
        }
        #endregion

        [ProducesResponseType(typeof(List<BatchReportCashSuspenseResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("cash/suspense")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> CreateReportCashSuspense([FromBody] BatchReportCashSuspenseRequest request)
        {
            var batchReport = request.ToDomain();
            batchReport.ReportName = ReportCashSuspenseLabel;

            var batchReportCharges = await _batchReportGateway
                .CreateAsync(batchReport)
                .ConfigureAwait(false);

            return Created("Report request created",
                           batchReportCharges.ToReportCashSuspenseResponse());
        }

        [ProducesResponseType(typeof(List<BatchReportCashSuspenseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("cash/suspense")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> ListReportCashSuspense()
        {
            var batchReportCharges = await _batchReportGateway
                .ListAsync(ReportCashSuspenseLabel).ConfigureAwait(false);

            if (batchReportCharges == null)
                return NotFound();
            return Ok(batchReportCharges.ToReportCashSuspenseResponse());
        }

        [ProducesResponseType(typeof(List<BatchReportCashImportResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("cash/import")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> CreateReportCashImport([FromBody] BatchReportCashImportRequest request)
        {
            var batchReport = request.ToDomain();
            batchReport.ReportName = ReportCashImportLabel;

            var batchReportCharges = await _batchReportGateway
                .CreateAsync(batchReport)
                .ConfigureAwait(false);

            return Created("Report request created",
                           batchReportCharges.ToReportCashImportResponse());
        }

        [ProducesResponseType(typeof(List<BatchReportCashImportResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("cash/import")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> ListReportCashImport()
        {
            var batchReportCharges = await _batchReportGateway
                .ListAsync(ReportCashImportLabel).ConfigureAwait(false);

            if (batchReportCharges == null)
                return NotFound();
            return Ok(batchReportCharges.ToReportCashImportResponse());
        }

        [ProducesResponseType(typeof(List<BatchReportAccountBalanceResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("balance")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> CreateReportAccountBalance([FromBody] BatchReportAccountBalanceRequest request)
        {
            var batchReport = request.ToDomain();
            batchReport.ReportName = ReportAccountBalanceByDateLabel;

            var batchReportAccountBalance = await _batchReportGateway
                .CreateAsync(batchReport)
                .ConfigureAwait(false);

            return Created("Report request created",
                           batchReportAccountBalance.ToReportAccountBalanceResponse());
        }

        [ProducesResponseType(typeof(List<BatchReportAccountBalanceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("balance")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> ListReportAccountBalance()
        {
            var batchReportAccountBalance = await _batchReportGateway
                .ListAsync(ReportAccountBalanceByDateLabel).ConfigureAwait(false);

            if (batchReportAccountBalance == null)
                return NotFound();
            return Ok(batchReportAccountBalance.ToReportAccountBalanceResponse());
        }

        [ProducesResponseType(typeof(List<BatchReportHousingBenefitAcademyResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("housingbenefit/academy")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> CreateReportHousingBenefitAcademy([FromBody] BatchReportHousingBenefitAcademyRequest request)
        {
            var batchReport = request.ToDomain();
            batchReport.ReportName = ReportHousingBenefitAcademyLabel;

            var batchReportHousingBenefitAcademy = await _batchReportGateway
                .CreateAsync(batchReport)
                .ConfigureAwait(false);

            return Created("Report request created",
                           batchReportHousingBenefitAcademy.ToReportHousingBenefitAcademyResponse());
        }

        [ProducesResponseType(typeof(List<BatchReportHousingBenefitAcademyResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("housingbenefit/academy")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> ListReportHousingBenefitAcademy()
        {
            var batchReportHousingBenefitAcademy = await _batchReportGateway
                .ListAsync(ReportHousingBenefitAcademyLabel).ConfigureAwait(false);

            if (batchReportHousingBenefitAcademy == null)
                return NotFound();
            return Ok(batchReportHousingBenefitAcademy.ToReportHousingBenefitAcademyResponse());
        }
    }
}
