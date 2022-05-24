using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace HousingFinanceInterimApi.V1.Controllers
{
    public class ReportController : BaseController
    {

        private readonly IReportChargesGateway _reportChargesGateway;
        private readonly IReportSuspenseAccountGateway _reportSuspenseAccountGateway;
        private readonly IReportCashImportGateway _reportCashImportGateway;

        public ReportController(IReportChargesGateway reportChargesGateway,
            IReportSuspenseAccountGateway reportSuspenseAccountGateway,
            IReportCashImportGateway reportCashImportGateway)
        {
            _reportChargesGateway = reportChargesGateway;
            _reportSuspenseAccountGateway = reportSuspenseAccountGateway;
            _reportCashImportGateway = reportCashImportGateway;
        }

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
    }
}
