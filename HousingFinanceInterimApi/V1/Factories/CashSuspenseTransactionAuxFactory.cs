using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class CashSuspenseTransactionAuxFactory
    {
        public static CashSuspenseTransactionAuxDomain ToDomain(this CashSuspenseTransactionAux cashSuspenseTransactionAux)
        {
            if (cashSuspenseTransactionAux == null)
                return null;

            return new CashSuspenseTransactionAuxDomain
            {
                Id = cashSuspenseTransactionAux.Id,
                RentAccount = cashSuspenseTransactionAux.RentAccount,
                Date = cashSuspenseTransactionAux.Date,
                Amount = cashSuspenseTransactionAux.Amount,
                NewRentAccount = cashSuspenseTransactionAux.NewRentAccount
            };
        }

        public static List<CashSuspenseTransactionAuxDomain> ToDomain(
            this ICollection<CashSuspenseTransactionAux> cashSuspenseTransactionAux)
        {
            return cashSuspenseTransactionAux?.Select(d => d.ToDomain()).ToList();
        }
    }
}
