using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace HousingFinanceInterimApi.V1.Controllers
{
    public class ReportController : BaseController
    {

        private readonly IReportChargesGateway _gateway;

        public ReportController(IReportChargesGateway gateway)
        {
            _gateway = gateway;
        }

        [HttpGet("charges")]
        public async Task<JsonResult> ListByYearAndRentGroup(int year, string rentGroup, string group)
        {
            if (!string.IsNullOrEmpty(rentGroup))
            {
                return Json(await _gateway.ListByYearAndRentGroupAsync(year, rentGroup).ConfigureAwait(false));
            }
            else if (!string.IsNullOrEmpty(group))
            {
                return Json(await _gateway.ListByGroupTypeAsync(year, group).ConfigureAwait(false));
            }
            else
            {
                return Json(await _gateway.ListByYearAsync(year).ConfigureAwait(false));
            }
        }
    }
}
