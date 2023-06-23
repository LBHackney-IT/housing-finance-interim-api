using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using HousingFinanceInterimApi.V1.Boundary.Response;
using Hackney.Core.Authorization;

namespace HousingFinanceInterimApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/batch")]
    [ApiVersion("1.0")]
    public class BatchController : BaseController
    {
        private readonly IGetBatchLogErrorUseCase _getBatchLogErrorUseCase;

        public BatchController(IGetBatchLogErrorUseCase getBatchLogErrorUseCase)
        {
            _getBatchLogErrorUseCase = getBatchLogErrorUseCase;
        }

        [ProducesResponseType(typeof(List<BatchLogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("errors")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> GetErrors()
        {
            var data = await _getBatchLogErrorUseCase.ExecuteAsync().ConfigureAwait(false);
            if (data == null)
                return NotFound();
            return Ok(data);

        }
    }
}
