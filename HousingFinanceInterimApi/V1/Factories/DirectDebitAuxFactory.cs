using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class DirectDebitAuxFactory
    {
        public static DirectDebitAuxDomain ToDomain(this DirectDebitAux directDebitAux)
        {
            if (directDebitAux == null)
                return null;

            return new DirectDebitAuxDomain
            {
                Id = directDebitAux.Id,
                RentAccount = directDebitAux.RentAccount,
                DueDay = directDebitAux.DueDay,
                Amount = directDebitAux.Amount,
                Timestamp = directDebitAux.Timestamp
            };
        }

        public static List<DirectDebitAuxDomain> ToDomain(
            this ICollection<DirectDebitAux> directDebitsAux)
        {
            return directDebitsAux?.Select(d => d.ToDomain()).ToList();
        }
    }
}
