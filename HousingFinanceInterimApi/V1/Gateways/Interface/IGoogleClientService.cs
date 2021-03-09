using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The google client service interface.
    /// </summary>
    public interface IGoogleClientService
    {

        /// <summary>
        /// Reads the file line data asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="mime">The MIME.</param>
        /// <returns>The file contents line by line.</returns>
        public Task<IList<string>> ReadFileLineDataAsync(string fileName, string fileId, string mime);

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
