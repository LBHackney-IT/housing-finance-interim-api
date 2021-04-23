using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Controllers
{

    public class TenancyController : BaseController
    {

        private readonly ITenancyGateway _gateway;

        public TenancyController(ITenancyGateway gateway)
        {
            _gateway = gateway;
        }

        [HttpGet]
        public async Task<JsonResult> Get(string tenancyAgreementRef, string rentAccount, string householdRef)
            => Json(await _gateway.GetAsync(tenancyAgreementRef, rentAccount, householdRef).ConfigureAwait(false));

        [HttpGet("transaction")]
        public async Task<JsonResult> GetTransactions(string tenancyAgreementRef, string rentAccount, string householdRef, int count, string order)
            => Json(await _gateway.GetTransactionsAsync(tenancyAgreementRef, rentAccount, householdRef, count, order).ConfigureAwait(false));
    }
}
