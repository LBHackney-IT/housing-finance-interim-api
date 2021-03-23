using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
        public async Task<JsonResult> Get(DateTime startDate, DateTime endDate)
            => Json(await _gateway.ListAsync(startDate, endDate).ConfigureAwait(false));

    }

}
