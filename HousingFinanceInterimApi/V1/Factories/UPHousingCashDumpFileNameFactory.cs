using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{

    /// <summary>
    /// The UP cash dump file name domain factory.
    /// </summary>
    public static class UPHousingCashDumpFileNameFactory
    {
        public static UPHousingCashDumpFileNameDomain ToDomain(this UPHousingCashDumpFileName housingCashDumpFileName)
        {
            if (housingCashDumpFileName == null)
                return null;

            return new UPHousingCashDumpFileNameDomain
            {
                Id = housingCashDumpFileName.Id,
                FileName = housingCashDumpFileName.FileName,
                IsSuccess = housingCashDumpFileName.IsSuccess,
                Timestamp = housingCashDumpFileName.Timestamp
            };
        }

        public static List<UPHousingCashDumpFileNameDomain> ToDomain(
            this ICollection<UPHousingCashDumpFileName> housingCashDumpFileName)
        {
            return housingCashDumpFileName?.Select(b => b.ToDomain()).ToList();
        }
    }
}
