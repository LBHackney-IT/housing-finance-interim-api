namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The operating balance entity.
    /// </summary>
    public class Transaction : BaseOperatingBalanceTransactionEntity
    {
        public decimal WeekMonth { get; set; }
    }
}
