using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace HousingFinanceInterimApi.V1.Controllers
{
    public class OperatingBalanceController : BaseController
    {

        private readonly IOperatingBalanceGateway _gateway;

        public OperatingBalanceController(IOperatingBalanceGateway gateway)
        {
            _gateway = gateway;
        }

        [HttpGet]
        public async Task<JsonResult> Get(DateTime? startDate, DateTime? endDate, int startWeek, int startYear, int endWeek, int endYear)
            =>
                Json(await _gateway.ListAsync(startDate, endDate, startWeek, startYear, endWeek, endYear).ConfigureAwait(false));
    }

}
