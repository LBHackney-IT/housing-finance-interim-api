using Microsoft.AspNetCore.Mvc;

namespace HousingFinanceInterimApi.V1.Boundary.Request
{
    public class UpdateAssetDetailsQuery
    {
        [FromRoute(Name = "propertyReference")]
        public string PropertyReference { get; set; }
    }
}
