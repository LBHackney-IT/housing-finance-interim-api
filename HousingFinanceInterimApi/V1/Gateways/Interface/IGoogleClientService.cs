using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The google client service interface.
    /// </summary>
    public interface IGoogleClientService
    {

        /// <summary>
        /// Ensures the user's shared Google folder exists.
        /// </summary>
        /// <param name="userEmail">The user email.</param>
        /// <param name="userGoogleId">The user google identifier.</param>
        /// <returns>
        /// A bool determining the success of the method.
        /// </returns>
        public Task<bool> EnsureUserFolderExistsAsync(string userEmail, string userGoogleId);

        /// <summary>
        /// Reads the given spreadsheet to a JSON file asynchronous.
        /// </summary>
        /// <param name="spreadSheetId">The spread sheet identifier.</param>
        /// <param name="sheetName">Name of the sheet to read.</param>
        /// <param name="sheetRange">The sheet range to read.</param>
        /// <param name="outputFileName">Name of the output file.</param>
        /// <returns>
        /// An async task.
        /// </returns>
        public Task ReadSheetToJsonAsync(string spreadSheetId, string sheetName, string sheetRange,
            string outputFileName);

    }

}
