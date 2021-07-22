using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Drive.v3.Data;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;

namespace HousingFinanceInterimApi.V1.UseCase
{

    /// <summary>
    /// The get files in google drive use case implementation.
    /// </summary>
    /// <seealso cref="IGetFilesInGoogleDriveUseCase" />
    public class GetFilesInGoogleDriveUseCase : IGetFilesInGoogleDriveUseCase
    {

        /// <summary>
        /// The google client service
        /// </summary>
        private readonly IGoogleClientService _googleClientService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFilesInGoogleDriveUseCase"/> class.
        /// </summary>
        /// <param name="googleClientService">The google client service.</param>
        public GetFilesInGoogleDriveUseCase(IGoogleClientService googleClientService)
        {
            _googleClientService = googleClientService;
        }

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="folderId">The folder identifier.</param>
        /// <returns>
        /// A list of google files.
        /// </returns>
        public async Task<IList<File>> ExecuteAsync(string folderId)
            => await _googleClientService.GetFilesInDriveAsync(folderId).ConfigureAwait(false);

    }

}
