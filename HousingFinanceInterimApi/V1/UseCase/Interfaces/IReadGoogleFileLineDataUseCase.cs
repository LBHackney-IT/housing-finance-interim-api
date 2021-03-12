using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The read Google file line data use case interface.
    /// </summary>
    public interface IReadGoogleFileLineDataUseCase
    {

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="mime">The MIME.</param>
        /// <returns>The file's content line by line.</returns>
        public Task<IList<string>> ExecuteAsync(string fileName, string fileId, string mime);

    }

}
