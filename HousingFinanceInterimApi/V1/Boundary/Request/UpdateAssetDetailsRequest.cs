namespace HousingFinanceInterimApi.V1.Boundary.Request
{
    public class UpdateAssetDetailsRequest
    {
        public string PostPreamble { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string PostCode { get; set; }
    }
}
