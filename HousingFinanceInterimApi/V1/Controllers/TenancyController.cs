using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore.Design;

namespace HousingFinanceInterimApi.V1.Controllers
{

    [EnableCors]
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
        public async Task<JsonResult> GetTransactions(string tenancyAgreementRef, string rentAccount, string householdRef,
            int count, string order, DateTime startDate, DateTime endDate)
        {
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                return Json(await _gateway
                    .GetTransactionsByDateAsync(tenancyAgreementRef, rentAccount, householdRef, startDate, endDate)
                    .ConfigureAwait(false));

            return Json(await _gateway.GetTransactionsAsync(tenancyAgreementRef, rentAccount, householdRef, count, order)
                .ConfigureAwait(false));
        }

    }

}
