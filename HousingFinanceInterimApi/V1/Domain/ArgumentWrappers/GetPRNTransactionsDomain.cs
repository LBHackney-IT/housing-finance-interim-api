
namespace HousingFinanceInterimApi.V1.Domain.ArgumentWrappers
{
    public class GetPRNTransactionsDomain
    {
        public string RentGroup { get; set; }
        public int FinancialYear { get; set; }
        public int StartWeekOrMonth { get; set; }
        public int EndWeekOrMonth { get; set; }
    }
}
