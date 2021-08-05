using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace HousingFinanceInterimApi.V1.Controllers
{
    public class PaymentController : BaseController
    {

        private readonly IPaymentGateway _gateway;

        public PaymentController(IPaymentGateway gateway)
        {
            _gateway = gateway;
        }

        [HttpGet]
        public async Task<JsonResult> Get(string tenancyAgreementRef, string rentAccount, string householdRef, int count, string order)
            => Json(await _gateway.ListAsync(tenancyAgreementRef, rentAccount, householdRef, count, order).ConfigureAwait(false));
    }

}
