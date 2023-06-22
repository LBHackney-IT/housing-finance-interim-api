using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using HousingFinanceInterimApi.V1.Infrastructure;
using Hackney.Core.Authorization;

namespace HousingFinanceInterimApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/operatingbalance")]
    [ApiVersion("1.0")]
    public class OperatingBalanceController : BaseController
    {

        private readonly IOperatingBalanceGateway _gateway;

        public OperatingBalanceController(IOperatingBalanceGateway gateway)
        {
            _gateway = gateway;
        }

        [ProducesResponseType(typeof(List<OperatingBalance>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("")]
        [AuthorizeEndpointByGroups("HOUSING_FINANCE_ALLOWED_GROUPS")]
        public async Task<IActionResult> Get(DateTime? startDate, DateTime? endDate, int startWeek, int startYear, int endWeek, int endYear)
        {
            var data = await _gateway.ListAsync(startDate, endDate, startWeek, startYear, endWeek, endYear).ConfigureAwait(false);
            if (data == null)
                return NotFound();
            return Ok(data);
        }
    }
}
