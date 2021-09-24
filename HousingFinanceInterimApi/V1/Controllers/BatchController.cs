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
    public class BatchController : BaseController
    {
        private readonly IGetBatchLogErrorUseCase _getBatchLogErrorUseCase;

        public BatchController(IGetBatchLogErrorUseCase getBatchLogErrorUseCase)
        {
            _getBatchLogErrorUseCase = getBatchLogErrorUseCase;
        }

        [HttpGet("errors")]
        public async Task<JsonResult> GetErrors()
        {
            return Json(await _getBatchLogErrorUseCase.ExecuteAsync().ConfigureAwait(false));
        }
    }
}
