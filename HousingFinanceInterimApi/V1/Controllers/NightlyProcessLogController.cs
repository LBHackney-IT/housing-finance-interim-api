using Hackney.Core.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/nightly-process-status")]
    [ApiVersion("1.0")]
    public class NightlyProcessLogController : BaseController
    {
        private readonly INightlyProcessLogUseCase _nightlyProcessLogUseCase;

        public NightlyProcessLogController(INightlyProcessLogUseCase nightlyProcessLogUseCase)
        {
            _nightlyProcessLogUseCase = nightlyProcessLogUseCase;
        }

        [ProducesResponseType(typeof(List<NightlyProcessLogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> GetNightlyProcessLogs([FromQuery] DateTime createdDate)
        {
            var logs = await _nightlyProcessLogUseCase.ExecuteAsync(createdDate).ConfigureAwait(false);

            if (logs == null || logs.Count == 0)
                return NotFound();

            return Ok(logs);
        }
    }
}
