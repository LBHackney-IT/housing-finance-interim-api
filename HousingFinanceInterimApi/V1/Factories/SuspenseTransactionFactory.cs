using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class SuspenseTransactionFactory
    {
        public static SuspenseTransactionAuxDomain ToDomain(this SuspenseTransaction cashSuspenseTransaction)
        {
            if (cashSuspenseTransaction == null)
                return null;

            return new SuspenseTransactionAuxDomain
            {
                Id = cashSuspenseTransaction.Id,
                RentAccount = cashSuspenseTransaction.RentAccount,
                Date = cashSuspenseTransaction.PaymentDate,
                Amount = cashSuspenseTransaction.Amount,
                NewRentAccount = cashSuspenseTransaction.NewRentAccount
            };
        }

        public static List<SuspenseTransactionAuxDomain> ToDomain(this List<SuspenseTransaction> cashSuspenseTransaction)
        {
            return cashSuspenseTransaction?.Select(d => d.ToDomain()).ToList();
        }
    }
}
