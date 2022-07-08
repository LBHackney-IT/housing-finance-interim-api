using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class SuspenseTransactionAuxFactory
    {
        public static SuspenseTransactionAuxDomain ToDomain(this SuspenseTransactionAux cashSuspenseTransactionAux)
        {
            if (cashSuspenseTransactionAux == null)
                return null;

            return new SuspenseTransactionAuxDomain
            {
                Id = cashSuspenseTransactionAux.Id,
                RentAccount = cashSuspenseTransactionAux.RentAccount,
                Date = cashSuspenseTransactionAux.Date,
                Amount = cashSuspenseTransactionAux.Amount,
                NewRentAccount = cashSuspenseTransactionAux.NewRentAccount
            };
        }

        public static List<SuspenseTransactionAuxDomain> ToDomain(
            this ICollection<SuspenseTransactionAux> cashSuspenseTransactionAux)
        {
            return cashSuspenseTransactionAux?.Select(d => d.ToDomain()).ToList();
        }
    }
}
