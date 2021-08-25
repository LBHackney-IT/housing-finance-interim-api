using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class ActionDiaryAuxFactory
    {
        public static ActionDiaryAuxDomain ToDomain(this ActionDiaryAux actionDiaryAux)
        {
            if (actionDiaryAux == null)
                return null;

            return new ActionDiaryAuxDomain
            {
                Id = actionDiaryAux.Id,
                TenancyAgreementRef = actionDiaryAux.TenancyAgreementRef,
                RentAccount = actionDiaryAux.RentAccount,
                ActionCode = actionDiaryAux.ActionCode,
                Action = actionDiaryAux.Action,
                ActionDate = actionDiaryAux.ActionDate,
                Username = actionDiaryAux.Username,
                ActionComment = actionDiaryAux.ActionComment,
                Balance = actionDiaryAux.Balance,
                Timestamp = actionDiaryAux.Timestamp
            };
        }

        public static List<ActionDiaryAuxDomain> ToDomain(
            this ICollection<ActionDiaryAux> actionsDiaryAux)
        {
            return actionsDiaryAux?.Select(d => d.ToDomain()).ToList();
        }
    }
}
