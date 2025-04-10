using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Controllers
{
    [Route("api/v1/tenancy-agreement")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TenancyAgreementController : ControllerBase
    {
        private readonly IUpdateTAUseCase _useCase;

        public TenancyAgreementController(IUpdateTAUseCase useCase)
        {
            _useCase = useCase;
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch]
        public async Task<IActionResult> DynamoDbStreamTrigger([FromQuery] string tagRef, [FromBody] UpdateTARequest request)
        {
            if (tagRef == null) return BadRequest("No tagRef query parameter provided");

            await _useCase.ExecuteAsync(tagRef, request).ConfigureAwait(false);

            return NoContent();
        }
    }
}
