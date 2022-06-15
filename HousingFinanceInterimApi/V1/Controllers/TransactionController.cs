using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Transactions;

namespace HousingFinanceInterimApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/transaction")]
    [ApiVersion("1.0")]
    public class TransactionController : BaseController
    {

        private readonly ITransactionGateway _gateway;

        public TransactionController(ITransactionGateway gateway)
        {
            _gateway = gateway;
        }

        [ProducesResponseType(typeof(List<Transaction>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("summary")]
        public async Task<IActionResult> Get(DateTime? startDate, DateTime? endDate)
        {
            var data = await _gateway.ListAsync(startDate, endDate).ConfigureAwait(false);
            if (data == null)
                return NotFound();
            return Ok(data);
        }
    }
}
