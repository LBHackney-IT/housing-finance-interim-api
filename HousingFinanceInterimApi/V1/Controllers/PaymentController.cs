using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/payment")]
    [ApiVersion("1.0")]
    public class PaymentController : BaseController
    {

        private readonly IPaymentGateway _gateway;

        public PaymentController(IPaymentGateway gateway)
        {
            _gateway = gateway;
        }

        [ProducesResponseType(typeof(List<Payment>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(string tenancyAgreementRef, string rentAccount, string householdRef, int count, string order)
        {
            var data = await _gateway.ListAsync(tenancyAgreementRef, rentAccount, householdRef, count, order).ConfigureAwait(false);
            if (data == null)
                return NotFound();
            return Ok(data);
        }
    }

}
