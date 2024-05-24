using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Drive.v3.Data;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The google client service interface.
    /// </summary>
    public interface IGoogleClientService
    {

        /// <summary>
        /// Gets the files in drive asynchronous.
        /// </summary>
        /// <param name="driveId">The drive identifier.</param>
        /// <param name="fieldsOverride">A custom list of fields to fetch from the google drive api response.</param>
        /// <returns>The list of files for the given drive.</returns>
        public Task<IList<Google.Apis.Drive.v3.Data.File>> GetFilesInDriveAsync(string driveId, string fieldsOverride = null);

        /// <summary>
        /// Gets the files in a drive folder asynchronously by name.
        /// </summary>
        /// <param name="driveId">The drive identifier.</param>
        /// <param name="fileName"></param>
        /// <returns>The list of files for the given drive.</returns>
        public Task<Google.Apis.Drive.v3.Data.File> GetFileByNameInDriveAsync(string driveId, string fileName);

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
        /// <returns>
        /// An async task.
        /// </returns>
        public Task<IList<_TEntity>> ReadSheetToEntitiesAsync<_TEntity>(string spreadSheetId, string sheetName,
            string sheetRange) where _TEntity : class;

        public Task CopyFileInDrive(string fileId, string destinationFolderId, string fileName);
        public Task<bool> RenameFileInDrive(string fileId, string newName);

        public Task<bool> UploadFileInDrive(MemoryStream memoryStream, string fileName, string folderId);

        public Task<bool> UploadCsvFile(List<string[]> table, string fileName, string folderId);

        public Task ClearSheetAsync(string spreadSheetId, string sheetName, string sheetRange);

        public Task UpdateSheetAsync(List<IList<object>> data, string spreadSheetId, string sheetName, string sheetRange, bool clearSheet = false);

        public Task DeleteFileInDrive(string fileId);

    }

}
