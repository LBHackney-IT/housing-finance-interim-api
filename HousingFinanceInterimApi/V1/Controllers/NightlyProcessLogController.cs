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

        /// <summary>
        /// Retrieves the status of the nightly HFS ingest processes for a specific date.
        /// </summary>
        /// <param name="createdDate">The date for which to retrieve nightly process logs</param>
        /// <returns>
        /// Queries the AWS CloudWatch log groups for the keyword "Error" on specified date and returns results in 3 cases.
        /// The results are filtered based on the log group names defined in the LogGroupUtility class.
        /// The returned are based on the following Result Cases:
        /// Case 1: Results exist for the keyword in the log group - Empty list, IsSuccess is false and Timestamp is not null
        /// Case 2: Results do not exist for the keyword in the log group - Populated List, IsSuccess is true and Timestamp is not null
        /// Case 3: No logs exist for the log group in the last 24 hours - Empty list, IsSuccess is null and Timestamp is null
        /// </returns>
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
