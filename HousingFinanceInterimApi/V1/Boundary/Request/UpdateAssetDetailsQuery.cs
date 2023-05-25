using Microsoft.AspNetCore.Mvc;

namespace HousingFinanceInterimApi.V1.Boundary.Request
{
    public class UpdateAssetDetailsQuery
    {
        [FromRoute(Name = "properyReference")]
        public string PropertyReference { get; set; }
    }
}
