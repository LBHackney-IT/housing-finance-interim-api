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
using Google.Apis.Upload;
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
        public async Task<IList<Google.Apis.Drive.v3.Data.File>> GetFilesInDriveAsync(string driveId)
        {
            FilesResource.ListRequest listRequest = _driveService.Files.List();
            listRequest.Q = $"'{driveId}' in parents";

            // Recursively get files from drive
            return await GetFilesInDrive(listRequest, null).ConfigureAwait(false);
        }

        public async Task<File> GetFilesInDriveAsync(string driveId, string fileName)
        {
            var files = await GetFilesInDriveAsync(driveId).ConfigureAwait(false);

            return files.Where(f => f.Name == fileName).FirstOrDefault();
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

        public Task<bool> RenameFileInDrive(string fileId, string newName)
        {
            File newFileName = new File();
            newFileName.Name = newName;


            var updateRequest = _driveService.Files.Update(newFileName, fileId);
            var renamedFile = updateRequest.Execute();

            return Task.FromResult(renamedFile.Name == newName);
        }

        public async Task DeleteFileInDrive(string fileId)
        {
            var deleteRequest = _driveService.Files.Delete(fileId);
            var deletedFile = await deleteRequest.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<bool> UploadFileInDrive(MemoryStream memoryStream, string fileName, string folderId)
        {
            IUploadProgress createdFile = null;
            try
            {

                LoggingHandler.LogInfo($"Uploading file");

                var path = "/tmp/tempfiles";
                var outputPath = $"{path}/{fileName}";

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (System.IO.File.Exists(outputPath))
                    System.IO.File.Delete(outputPath);

                File newFile = new File()
                {
                    Name = fileName,
                    MimeType = "text/plain",
                    Parents = new List<string> { folderId }
                };

                using (var stream = new FileStream(outputPath, FileMode.Create))
                {
                    memoryStream.WriteTo(stream);
                    var createRequest = _driveService.Files.Create(newFile, stream, "text/plain");
                    createdFile = await createRequest.UploadAsync().ConfigureAwait(false);
                }

                LoggingHandler.LogInfo($"Upload progress: { JsonConvert.SerializeObject(createdFile) }");

                return createdFile.Status == UploadStatus.Completed;
            }
            catch (Exception exc)
            {
                LoggingHandler.LogError($"Error uploading file");
                LoggingHandler.LogError($"Upload progress: { JsonConvert.SerializeObject(createdFile) }");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }

        public async Task<bool> UploadCsvFile(List<string[]> table, string fileName, string folderId)
        {
            IUploadProgress createdFile = null;
            try
            {
                LoggingHandler.LogInfo($"Uploading csv file");

                var path = "/tmp/tempfiles";
                var outputPath = $"{path}/{fileName}";

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (System.IO.File.Exists(outputPath))
                    System.IO.File.Delete(outputPath);

                using (var w = new StreamWriter(outputPath))
                {
                    foreach (var row in table)
                    {
                        var newRow = string.Join(";", row);
                        await w.WriteLineAsync(newRow).ConfigureAwait(false);
                        await w.FlushAsync().ConfigureAwait(false);
                    }
                }

                File newFile = new File()
                {
                    Name = fileName,
                    MimeType = "text/csv",
                    Parents = new List<string> { folderId }
                };

                using (var stream = new System.IO.FileStream(outputPath, FileMode.Open))
                {
                    var createRequest = _driveService.Files.Create(newFile, stream, "text/csv");
                    createdFile = await createRequest.UploadAsync().ConfigureAwait(false);
                }

                LoggingHandler.LogInfo($"Upload progress: { JsonConvert.SerializeObject(createdFile) }");

                return createdFile.Status == UploadStatus.Completed;
            }
            catch (Exception exc)
            {
                LoggingHandler.LogError($"Error uploading csv file");
                LoggingHandler.LogError($"Upload progress: { JsonConvert.SerializeObject(createdFile) }");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }

        #endregion

        #region Google Sheets
        public async Task UpdateSheetAsync(List<IList<object>> data, string spreadSheetId, string sheetName, string sheetRange, bool clearSheet = false)
        {
            if (clearSheet)
                await ClearSheetAsync(spreadSheetId, sheetName, sheetRange).ConfigureAwait(false);

            string valueInputOption = "USER_ENTERED";

            // The new values to apply to the spreadsheet.
            List<ValueRange> updateData = new List<ValueRange>();
            var dataValueRange = new ValueRange();
            dataValueRange.Range = $"{sheetName}!{sheetRange}";
            dataValueRange.Values = data;
            updateData.Add(dataValueRange);

            Data.BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest();
            requestBody.ValueInputOption = valueInputOption;
            requestBody.Data = updateData;

            var request = _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, spreadSheetId);

            request.Execute();
        }

        public Task ClearSheetAsync(string spreadSheetId, string sheetName, string sheetRange)
        {
            var requestBody = new ClearValuesRequest();

            var requestClear =
                _sheetsService.Spreadsheets.Values.Clear(requestBody, spreadSheetId, $"{sheetName}!{sheetRange}");

            requestClear.Execute();
            return Task.CompletedTask;
        }

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
                LoggingHandler.LogInfo($"No data found. Spreadsheet id: {spreadSheetId}, sheet name: {sheetName}, sheet range: {sheetRange}");
                return null;
            }
            LoggingHandler.LogInfo($"Rows {values.Count} found");

            // Get the headers
            IList<string> headers = values.First().Select(cell => cell.ToString()).ToList();
            IList<object> rowObjects = new List<object>();
            LoggingHandler.LogInfo($"Writing row values to objects, {headers.Count} headers found");

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
                LoggingHandler.LogInfo($"Writing values to objects and serializing");
                string convertedJson = JsonConvert.SerializeObject(rowObjects);
                var entities = JsonConvert.DeserializeObject<IList<_TEntity>>(convertedJson);

                return entities;
            }
            catch (Exception exc)
            {
                LoggingHandler.LogInfo($"Error writing values to objects and serializing");
                LoggingHandler.LogInfo(exc.ToString());

                throw;
            }
        }

        #endregion

        #endregion

    }

}
