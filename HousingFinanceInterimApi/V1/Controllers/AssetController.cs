using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Controllers
{
    [Route("api/v1/asset")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IAssetGateway _assetGateway;

        public AssetController(IAssetGateway assetGateway)
        {
            _assetGateway = assetGateway;
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch]
        [Route("{propertyReference}")]
        public async Task<IActionResult> UpdateAssetDetails([FromRoute] UpdateAssetDetailsQuery query, [FromBody] UpdateAssetDetailsRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.AddressLine1))
            {
                return BadRequest($"The value for {nameof(request.AddressLine1)} cannot be empty");
            }

            await _assetGateway.UpdateAssetDetails(query, request).ConfigureAwait(false);

            return NoContent();
        }
    }
}
