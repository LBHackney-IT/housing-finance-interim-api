using Google.Apis.Drive.v3.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The get files in google drive use case.
    /// </summary>
    public interface IGetFilesInGoogleDriveUseCase
    {

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="folderId">The folder identifier.</param>
        /// <returns>A list of google files.</returns>
        public Task<IList<File>> ExecuteAsync(string folderId);

    }

}
