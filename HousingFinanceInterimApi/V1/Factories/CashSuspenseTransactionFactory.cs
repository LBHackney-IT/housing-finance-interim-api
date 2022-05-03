using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class CashSuspenseTransactionFactory
    {
        public static CashSuspenseTransactionAuxDomain ToDomain(this CashSuspenseTransaction cashSuspenseTransaction)
        {
            if (cashSuspenseTransaction == null)
                return null;

            return new CashSuspenseTransactionAuxDomain
            {
                Id = cashSuspenseTransaction.Id,
                RentAccount = cashSuspenseTransaction.RentAccount,
                Date = cashSuspenseTransaction.PaymentDate,
                Amount = cashSuspenseTransaction.Amount,
                NewRentAccount = cashSuspenseTransaction.NewRentAccount
            };
        }

        public static List<CashSuspenseTransactionAuxDomain> ToDomain(this List<CashSuspenseTransaction> cashSuspenseTransaction)
        {
            return cashSuspenseTransaction?.Select(d => d.ToDomain()).ToList();
        }
    }
}
