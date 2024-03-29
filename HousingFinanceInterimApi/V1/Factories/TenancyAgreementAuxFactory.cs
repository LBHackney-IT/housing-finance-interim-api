using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class TenancyAgreementAuxFactory
    {
        public static TenancyAgreementAuxDomain ToDomain(this TenancyAgreementAux tenancyAgreementAux)
        {
            if (tenancyAgreementAux == null)
                return null;

            return new TenancyAgreementAuxDomain
            {
                Id = tenancyAgreementAux.Id,
                PaymentRef = tenancyAgreementAux.PaymentRef,
                Tenure = tenancyAgreementAux.Tenure,
                StartDate = tenancyAgreementAux.StartDate,
                EndDate = tenancyAgreementAux.EndDate,
                PropertyRef = tenancyAgreementAux.PropertyRef,
                ShortAddress = tenancyAgreementAux.ShortAddress,
                Address = tenancyAgreementAux.Address,
                PostCode = tenancyAgreementAux.PostCode,
                NumBedrooms = tenancyAgreementAux.NumBedrooms,
                Title = tenancyAgreementAux.Title,
                Forename = tenancyAgreementAux.Forename,
                Surname = tenancyAgreementAux.Surname,
                DateOfBirth = tenancyAgreementAux.DateOfBirth,
                Timestamp = tenancyAgreementAux.TimeStamp
            };
        }

        public static List<TenancyAgreementAuxDomain> ToDomain(
            this ICollection<TenancyAgreementAux> tenancyAgreementsAux)
        {
            return tenancyAgreementsAux?.Select(t => t.ToDomain()).ToList();
        }
    }
}
