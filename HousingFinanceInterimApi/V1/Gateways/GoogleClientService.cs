using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Handlers;
using File = Google.Apis.Drive.v3.Data.File;
using Data = Google.Apis.Sheets.v4.Data;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The google client service implementation.
    /// </summary>
    /// <seealso cref="IGoogleClientService" />
    public class GoogleClientService : IGoogleClientService
    {

        #region Private

        /// <summary>
        /// The service initializer
        /// </summary>
        private readonly BaseClientService.Initializer _initializer;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        #region Drive service

        /// <summary>
        /// The drive service backing variable
        /// </summary>
        private DriveService _driveServiceBacking;

        /// <summary>
        /// Gets the drive service.
        /// </summary>
        private DriveService _driveService => _driveServiceBacking ??= new DriveService(_initializer);

        #endregion

        #region Sheets service

        /// <summary>
        /// The sheets service backing variable
        /// </summary>
        private SheetsService _sheetsServiceBacking;

        /// <summary>
        /// Gets the sheets service.
        /// </summary>
        private SheetsService _sheetsService => _sheetsServiceBacking ??= new SheetsService(_initializer);

        #endregion

        #endregion

        #region Public

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleClientService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="initializer">The initializer.</param>
        public GoogleClientService(ILogger logger, BaseClientService.Initializer initializer)
        {
            _logger = logger;
            _initializer = initializer;
        }

        #region Google Drive

        /// <summary>
        /// Reads the file line data asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="mime">The MIME.</param>
        /// <returns>
        /// The file contents line by line.
        /// </returns>
        public async Task<IList<string>> ReadFileLineDataAsync(string fileName, string fileId, string mime)
        {
            FilesResource.GetRequest request = _driveService.Files.Get(fileId);
            IList<string> results = new List<string>();

            await using MemoryStream stream = new MemoryStream();
            IDownloadProgress progress = await request.DownloadAsync(stream).ConfigureAwait(false);

            if (progress.Status == DownloadStatus.Completed)
            {
                if (!Directory.Exists("/tmp/tempfiles"))
                {
                    Directory.CreateDirectory("/tmp/tempfiles");
                }

                string outputPath = $"/tmp/tempfiles/{fileName}";

                await using (FileStream file = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    stream.WriteTo(file);
                }

                if (System.IO.File.Exists(outputPath))
                {
                    results = await System.IO.File.ReadAllLinesAsync(outputPath).ConfigureAwait(false);
                    System.IO.File.Delete(outputPath);
                }

                return results;
            }

            return results;
        }

        /// <summary>
        /// Gets the files in drive asynchronous.
        /// </summary>
        /// <param name="driveId">The drive identifier.</param>
        /// <returns>
        /// The list of files for the given drive.
        /// </returns>
        public async Task<IList<File>> GetFilesInDriveAsync(string driveId)
        {
            FilesResource.ListRequest listRequest = _driveService.Files.List();
            listRequest.Q = $"'{driveId}' in parents";

            // Recursively get files from drive
            return await GetFilesInDrive(listRequest, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Recursively gets the files in the given drive for the given request.
        /// </summary>
        /// <param name="listRequest">The list request.</param>
        /// <param name="nextPage">The next page.</param>
        /// <returns>The full list of drive files.</returns>
        private static async Task<IList<File>> GetFilesInDrive(FilesResource.ListRequest listRequest, string nextPage)
        {
            var results = new List<File>();

            if (!string.IsNullOrWhiteSpace(nextPage))
            {
                listRequest.PageToken = nextPage;
            }

            FileList requestResult = await listRequest.ExecuteAsync().ConfigureAwait(false);

            if (requestResult.Files?.Any() ?? false)
            {
                results.AddRange(requestResult.Files);

                if (!string.IsNullOrWhiteSpace(requestResult.NextPageToken))
                {
                    results.AddRange(
                        await GetFilesInDrive(listRequest, requestResult.NextPageToken).ConfigureAwait(false));
                }
            }

            return results;
        }

        public async Task<bool> RenameFileInDrive(string fileId, string newName)
        {
            File newFileName = new File();
            newFileName.Name = newName;


            var updateRequest = _driveService.Files.Update(newFileName, fileId);
            var renamedFile = updateRequest.Execute();

            return renamedFile.Name == newName;
        }

        #endregion

        #region Google Sheets

        public async Task<IList<_TEntity>> ReadSheetToEntitiesAsync<_TEntity>(string spreadSheetId, string sheetName,
            string sheetRange) where _TEntity : class
        {
            //await UpdateSheet(spreadSheetId, sheetName, "Z1").ConfigureAwait(false);
            //await ClearSheet(spreadSheetId, sheetName, "Z1").ConfigureAwait(false);

            SpreadsheetsResource.ValuesResource.GetRequest getter =
                _sheetsService.Spreadsheets.Values.Get(spreadSheetId, $"{sheetName}!{sheetRange}");

            ValueRange response = await getter.ExecuteAsync().ConfigureAwait(false);
            IList<IList<object>> values = response.Values;

            if (values == null || !values.Any())
            {
                LoggingHandler.LogInfo($"NO DATA FOUND. SPREADSHEET ID: {spreadSheetId}, SHEET NAME: {sheetName}, SHEET RANGE: {sheetRange}");
                return null;
            }
            LoggingHandler.LogInfo($"ROWS {values.Count} FOUND");

            // Get the headers
            IList<string> headers = values.First().Select(cell => cell.ToString()).ToList();
            IList<object> rowObjects = new List<object>();
            LoggingHandler.LogInfo($"WRITING ROW VALUES TO OBJECTS, {headers.Count} HEADERS FOUND");

            // For each row of actual data
            foreach (var row in values.Skip(1))
            {
                int cellIterator = 0;
                int rowCellCount = row.Count;
                dynamic rowItem = new ExpandoObject();
                var rowItemAccessor = rowItem as IDictionary<string, object>;

                // Add the cell values using the headers as properties
                foreach (string header in headers)
                {
                    if (cellIterator < rowCellCount)
                    {
                        string propertyName = string.IsNullOrWhiteSpace(header)
                            ? $"Header{cellIterator}"
                            : header;

                        // Assign the value to this property
                        rowItemAccessor[propertyName] = row[cellIterator++];
                    }
                }
                rowObjects.Add(rowItem);
            }


            try
            {
                LoggingHandler.LogInfo($"WRITING VALUES TO OBJECTS AND SERIALIZING");
                string convertedJson = JsonConvert.SerializeObject(rowObjects);
                var entities = JsonConvert.DeserializeObject<IList<_TEntity>>(convertedJson);

                return entities;
            }
            catch (Exception exc)
            {
                LoggingHandler.LogInfo($"ERROR WRITING VALUES TO OBJECTS AND SERIALIZING");
                LoggingHandler.LogInfo(exc.ToString());

                throw;
            }
        }

        public async Task UpdateSheet(string spreadSheetId, string sheetName, string sheetRange)
        {
            // How the input data should be interpreted.
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption =
                (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum) 1;  // TODO: Update placeholder value.

            // TODO: Assign values to desired properties of `requestBody`. All existing
            // properties will be replaced:
            Data.ValueRange requestBody = new Data.ValueRange();
            var oblist = new List<object>() { "DD" };
            requestBody.Values = new List<IList<object>> { oblist };

            SpreadsheetsResource.ValuesResource.UpdateRequest request = _sheetsService.Spreadsheets.Values.Update(requestBody, spreadSheetId, $"{sheetName}!{sheetRange}");
            request.ValueInputOption = valueInputOption;

            Data.UpdateValuesResponse response = await request.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task ClearSheet(string spreadSheetId, string sheetName, string sheetRange)
        {
            Data.ClearValuesRequest requestBody = new Data.ClearValuesRequest();

            SpreadsheetsResource.ValuesResource.ClearRequest request = _sheetsService.Spreadsheets.Values.Clear(requestBody, spreadSheetId, $"{sheetName}!{sheetRange}");

            // To execute asynchronously in an async method, replace `request.Execute()` as shown:
            Data.ClearValuesResponse response = await request.ExecuteAsync().ConfigureAwait(false);
        }

        #endregion

        #endregion

    }

}
