using HousingFinanceInterimApi.V1.Gateways.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;

namespace HousingFinanceInterimApi.V1.UseCase
{

    /// <summary>
    /// The read Google file line data use case implementation.
    /// </summary>
    /// <seealso cref="IReadGoogleFileLineDataUseCase" />
    public class ReadGoogleFileLineDataUseCase : IReadGoogleFileLineDataUseCase
    {

        /// <summary>
        /// The google client service
        /// </summary>
        private readonly IGoogleClientService _googleClientService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadGoogleFileLineDataUseCase"/> class.
        /// </summary>
        /// <param name="googleClientService">The google client service.</param>
        public ReadGoogleFileLineDataUseCase(IGoogleClientService googleClientService)
        {
            _googleClientService = googleClientService;
        }

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="mime">The MIME.</param>
        /// <returns>
        /// The file's content line by line.
        /// </returns>
        public async Task<IList<string>> ExecuteAsync(string fileName, string fileId, string mime)
            => await _googleClientService.ReadFileLineDataAsync(fileName, fileId, mime).ConfigureAwait(false);

    }

}
