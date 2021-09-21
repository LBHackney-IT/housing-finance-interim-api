using HousingFinanceInterimApi.V1.Infrastructure;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The UP housing cash file name gateway interface.
    /// </summary>
    public interface IUPHousingCashDumpFileNameGateway
    {

        /// <summary>
        /// Gets the given file by the given file name asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>An instance of <see cref="UPHousingCashDumpFileName"/> or null if no record found.</returns>
        public Task<UPHousingCashDumpFileName> GetAsync(string fileName);

        /// <summary>
        /// Creates a UP Cash dump file name entry for the given file name asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
        /// <returns>The created instance of <see cref="UPHousingCashDumpFileName"/></returns>
        public Task<UPHousingCashDumpFileNameDomain> CreateAsync(string fileName, bool isSuccess = false);

        /// <summary>
        /// Sets the given file name entry to success asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <returns>A bool determining the success of the method.</returns>
        public Task<bool> SetToSuccessAsync(long fileId);

        public Task<UPHousingCashDumpFileNameDomain> GetProcessedFileByName(string fileName);
    }

}
