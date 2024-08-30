using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class UPCashDumpFileNameFactory
    {
        public static UPCashDumpFileNameDomain ToDomain(this UPCashDumpFileName cashDumpFileName)
        {
            if (cashDumpFileName == null)
                return null;

            return new UPCashDumpFileNameDomain
            {
                Id = cashDumpFileName.Id.Value,
                FileName = cashDumpFileName.FileName,
                IsSuccess = cashDumpFileName.IsSuccess,
                Timestamp = cashDumpFileName.Timestamp
            };
        }

        public static List<UPCashDumpFileNameDomain> ToDomain(
            this ICollection<UPCashDumpFileName> cashDumpFileName)
        {
            return cashDumpFileName?.Select(b => b.ToDomain()).ToList();
        }
    }
}
