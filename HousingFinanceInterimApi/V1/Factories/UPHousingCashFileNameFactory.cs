using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{

    /// <summary>
    /// The UP cash dump file name domain factory.
    /// </summary>
    public static class UPHousingCashFileNameFactory
    {

        /// <summary>
        /// Converts the given file names to domain objects.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// The list of file name domain objects.
        /// </returns>
        public static UPHousingCashFileNameDomain ToDomain(UPHousingCashDumpFileName fileName)
        {
            if (fileName == null)
                return null;

            return new UPHousingCashFileNameDomain
            {
                Id = fileName.Id,
                FileName = fileName.FileName,
                IsSuccess = fileName.IsSuccess,
                Timestamp = fileName.Timestamp
            };
        }
    }

}
