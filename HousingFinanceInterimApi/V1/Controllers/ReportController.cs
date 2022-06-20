using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Factories;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.Core;

namespace HousingFinanceInterimApi.V1.Controllers
{
    public class ReportController : BaseController
    {

        private readonly IReportChargesGateway _reportChargesGateway;
        private readonly IReportSuspenseAccountGateway _reportSuspenseAccountGateway;
        private readonly IReportCashImportGateway _reportCashImportGateway;
        private readonly IBatchReportAccountBalanceGateway _batchReportAccountBalanceGateway;

        // Information to generate pre-signed object URL.
        private readonly string _bucketName = "academy-cashfile-sync-dev";


        public ReportController(IReportChargesGateway reportChargesGateway,
            IReportSuspenseAccountGateway reportSuspenseAccountGateway,
            IReportCashImportGateway reportCashImportGateway,
            IBatchReportAccountBalanceGateway batchReportAccountBalanceGateway)
        {
            _reportChargesGateway = reportChargesGateway;
            _reportSuspenseAccountGateway = reportSuspenseAccountGateway;
            _reportCashImportGateway = reportCashImportGateway;
            _batchReportAccountBalanceGateway = batchReportAccountBalanceGateway;
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
        [HttpGet("charges")]
        public async Task<JsonResult> ListChargesByYearAndRentGroup(int year, string rentGroup, string group)
        {
            if (!string.IsNullOrEmpty(rentGroup))
            {
                return Json(await _reportChargesGateway.ListByYearAndRentGroupAsync(year, rentGroup).ConfigureAwait(false));
            }
            else if (!string.IsNullOrEmpty(group))
            {
                return Json(await _reportChargesGateway.ListByGroupTypeAsync(year, group).ConfigureAwait(false));
            }
            else
            {
                return Json(await _reportChargesGateway.ListByYearAsync(year).ConfigureAwait(false));
            }
        }

        [HttpGet("cash/suspense")]
        public async Task<JsonResult> ListCashSuspenseByYearAndType(int year, string suspenseAccountType)
        {
            return Json(await _reportSuspenseAccountGateway
                .ListCashSuspenseByYearAndTypeAsync(year, suspenseAccountType).ConfigureAwait(false));
        }

        [HttpGet("cash/import")]
        public async Task<JsonResult> ListCashImportByDate(DateTime startDate, DateTime endDate)
        {
            return Json(await _reportCashImportGateway
                .ListCashImportByDateAsync(startDate, endDate).ConfigureAwait(false));
        }

        [HttpPost("balance")]
        public async Task<JsonResult> CreateReportAccountBalance([FromBody] BatchReportAccountBalanceRequest request)
        {
            return Json(await _batchReportAccountBalanceGateway
                .CreateAsync(request.ToDomain()).ConfigureAwait(false));
        }

        [HttpGet("balance")]
        public async Task<JsonResult> ListReportAccountBalance()
        {
            return Json(await _batchReportAccountBalanceGateway
                .ListAsync().ConfigureAwait(false));
        }
    }
}
