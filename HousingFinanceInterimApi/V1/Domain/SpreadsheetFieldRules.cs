namespace HousingFinanceInterimApi.V1.Domain
{
    public static class SpreadsheetFieldRules
    {
        public static bool IsAccountReference(string value)
        {
            var length = value?.Trim().Length ?? 0;
            return length >= 4 && length <= 20;
        }

        public static bool IsTransactionType(string value)
        {
            var length = value?.Trim().Length ?? 0;
            return length >= 1 && length <= 3;
        }
    }
}
