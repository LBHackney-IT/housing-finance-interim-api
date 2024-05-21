
namespace HousingFinanceInterimApi.V1.Infrastructure
{
    /// <summary>
    /// Transaction model used for retrieving transactions for the
    /// operating balances report filtered by rentgroup (and period fields) 
    /// </summary>
    public class PRNTransactionEntity : BaseOperatingBalanceTransactionEntity
    {
        public string RentAccount { get; set; }
    }
}
