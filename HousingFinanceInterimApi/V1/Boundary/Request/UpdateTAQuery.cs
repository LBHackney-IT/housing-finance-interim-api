using Microsoft.AspNetCore.Mvc;

namespace HousingFinanceInterimApi.V1.Boundary.Request
{
    public class UpdateTAQuery
    {
        [FromRoute(Name = "tenancyRef")]
        public string TenancyAgreementRef { get; set; }


    }
}
