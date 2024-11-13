using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Controllers
{
    [Route("api/v1/tenancy-agreement")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UpdateTAController : ControllerBase
    {
        private readonly IUpdateTAGateway _gateway;

        public UpdateTAController(IUpdateTAGateway gateway)
        {
            _gateway = gateway;
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch]
        [Route("{propertyReference}")]
        public async Task<IActionResult> UpdateTA([FromRoute] UpdateTAQuery query, [FromBody] UpdateTARequest request)
        {
            await _gateway.UpdateTADetails(query, request).ConfigureAwait(false);

            return NoContent();
        }
    }
}
