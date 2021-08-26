using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class UPHousingCashLoadSuspenseAccountsFactory
    {
        public static UPHousingCashLoadSuspenseAccountsDomain ToDomain(this UPHousingCashLoadSuspenseAccounts housingCashLoadSuspenseAccount)
        {
            if (housingCashLoadSuspenseAccount == null)
                return null;

            return new UPHousingCashLoadSuspenseAccountsDomain
            {
                Id = housingCashLoadSuspenseAccount.Id,
                RentAccount = housingCashLoadSuspenseAccount.RentAccount,
                NewRentAccount = housingCashLoadSuspenseAccount.NewRentAccount,
                UPHousingCashDumpId = housingCashLoadSuspenseAccount.UPHousingCashDumpId,
                AcademyClaimRef = housingCashLoadSuspenseAccount.AcademyClaimRef,
                Date = housingCashLoadSuspenseAccount.Date,
                column2 = housingCashLoadSuspenseAccount.column2,
                value1 = housingCashLoadSuspenseAccount.value1,
                value2 = housingCashLoadSuspenseAccount.value2,
                value3 = housingCashLoadSuspenseAccount.value3,
                value4 = housingCashLoadSuspenseAccount.value4,
                value5 = housingCashLoadSuspenseAccount.value5,
                IsResolved = housingCashLoadSuspenseAccount.IsResolved,
                Timestamp = housingCashLoadSuspenseAccount.Timestamp
            };
        }

        public static List<UPHousingCashLoadSuspenseAccountsDomain> ToDomain(
            this ICollection<UPHousingCashLoadSuspenseAccounts> housingCashLoadSuspenseAccounts)
        {
            return housingCashLoadSuspenseAccounts?.Select(h => h.ToDomain()).ToList();
        }

        public static UPHousingCashLoadSuspenseAccountsResponse ToResponse(this UPHousingCashLoadSuspenseAccountsDomain housingCashLoadSuspenseAccount)
        {
            if (housingCashLoadSuspenseAccount == null)
                return null;

            return new UPHousingCashLoadSuspenseAccountsResponse
            {
                Id = housingCashLoadSuspenseAccount.Id,
                RentAccount = housingCashLoadSuspenseAccount.RentAccount,
                AcademyClaimRef = housingCashLoadSuspenseAccount.AcademyClaimRef,
                Date = housingCashLoadSuspenseAccount.Date,
                Amount = housingCashLoadSuspenseAccount.value1
            };
        }

        public static List<UPHousingCashLoadSuspenseAccountsResponse> ToResponse(
            this ICollection<UPHousingCashLoadSuspenseAccountsDomain> housingCashLoadSuspenseAccounts)
        {
            return housingCashLoadSuspenseAccounts?.Select(h => h.ToResponse()).ToList();
        }
    }
}
