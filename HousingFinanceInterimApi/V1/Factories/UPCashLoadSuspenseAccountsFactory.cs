using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class UPCashLoadSuspenseAccountsFactory
    {
        public static UPCashLoadSuspenseAccountsDomain ToDomain(this UPCashLoadSuspenseAccounts cashLoadSuspenseAccount)
        {
            if (cashLoadSuspenseAccount == null)
                return null;

            return new UPCashLoadSuspenseAccountsDomain
            {
                Id = cashLoadSuspenseAccount.Id,
                RentAccount = cashLoadSuspenseAccount.RentAccount,
                NewRentAccount = cashLoadSuspenseAccount.NewRentAccount,
                UPCashDumpId = cashLoadSuspenseAccount.UPCashDumpId,
                DatePaid = cashLoadSuspenseAccount.DatePaid,
                AmountPaid = cashLoadSuspenseAccount.AmountPaid,
                CivicaCode = cashLoadSuspenseAccount.CivicaCode,
                MethodOfPayment = cashLoadSuspenseAccount.MethodOfPayment,
                PaymentSource = cashLoadSuspenseAccount.PaymentSource,
                IsResolved = cashLoadSuspenseAccount.IsResolved,
                Timestamp = cashLoadSuspenseAccount.Timestamp
            };
        }

        public static List<UPCashLoadSuspenseAccountsDomain> ToDomain(
            this ICollection<UPCashLoadSuspenseAccounts> cashLoadSuspenseAccounts)
        {
            return cashLoadSuspenseAccounts?.Select(c => c.ToDomain()).ToList();
        }

        public static UPCashLoadSuspenseAccountsResponse ToResponse(this UPCashLoadSuspenseAccountsDomain cashLoadSuspenseAccount)
        {
            if (cashLoadSuspenseAccount == null)
                return null;

            return new UPCashLoadSuspenseAccountsResponse
            {
                Id = cashLoadSuspenseAccount.Id,
                RentAccount = cashLoadSuspenseAccount.RentAccount,
                DatePaid = cashLoadSuspenseAccount.DatePaid,
                AmountPaid = cashLoadSuspenseAccount.AmountPaid,
                CivicaCode = cashLoadSuspenseAccount.CivicaCode,
                MethodOfPayment = cashLoadSuspenseAccount.MethodOfPayment,
                PaymentSource = cashLoadSuspenseAccount.PaymentSource
            };
        }

        public static List<UPCashLoadSuspenseAccountsResponse> ToResponse(
            this ICollection<UPCashLoadSuspenseAccountsDomain> cashLoadSuspenseAccounts)
        {
            return cashLoadSuspenseAccounts?.Select(c => c.ToResponse()).ToList();
        }
    }
}
