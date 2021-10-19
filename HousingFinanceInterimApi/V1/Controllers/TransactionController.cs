using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace HousingFinanceInterimApi.V1.Controllers
{
    public class TransactionController : BaseController
    {

        private readonly ITransactionGateway _gateway;

        public TransactionController(ITransactionGateway gateway)
        {
            _gateway = gateway;
        }

        [HttpGet("summary")]
        public async Task<JsonResult> Get(DateTime? startDate, DateTime? endDate)
            =>
                Json(await _gateway.ListAsync(startDate, endDate).ConfigureAwait(false));
    }
}
