using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Cors;

namespace HousingFinanceInterimApi.V1.Controllers
{
    public class SuspenseAccountsController : BaseController
    {
        private readonly IGetSuspenseAccountsUseCase _getSuspenseAccountsUseCase;

        public SuspenseAccountsController(IGetSuspenseAccountsUseCase getSuspenseAccountsUseCase)
        {
            _getSuspenseAccountsUseCase = getSuspenseAccountsUseCase;
        }

        [HttpGet("cashfiles")]
        public async Task<JsonResult> GetCashFilesSuspenseAccounts()
        {
            return Json(await _getSuspenseAccountsUseCase.ListCashFileSuspenseAccountsAsync().ConfigureAwait(false));
        }

        [HttpGet("housingfiles")]
        public async Task<JsonResult> GetHousingFilesSuspenseAccounts()
        {
            return Json(await _getSuspenseAccountsUseCase.ListHousingFileSuspenseAccountsAsync().ConfigureAwait(false));
        }
    }
}
