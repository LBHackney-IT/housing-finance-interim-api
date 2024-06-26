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
using FileDescription = Google.Apis.Drive.v3.Data.File;
using Data = Google.Apis.Sheets.v4.Data;
using HousingFinanceInterimApi.V1.Domain;
using static Google.Apis.Drive.v3.FilesResource;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class GoogleClientService : IGoogleClientService
    {
        private readonly ILogger _logger;
        private DriveService _driveService;
        private SheetsService _sheetsService;

        public GoogleClientService(ILogger logger, BaseClientService.Initializer initializer)
        {
            _logger = logger;
            _driveService = new DriveService(initializer);
            _sheetsService = new SheetsService(initializer);
        }

        public GoogleClientService(ILogger logger, DriveService driveService, SheetsService sheetsService)
        {
            _logger = logger;
            _driveService = driveService;
            _sheetsService = sheetsService;
        }

        #region Google Drive

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

        public async Task<IList<Google.Apis.Drive.v3.Data.File>> GetFilesInDriveAsync(string driveId, string fieldsOverride = null)
        {
            FilesResource.ListRequest listRequest = _driveService.Files.List();
            listRequest.Q = $"'{driveId}' in parents";
            if (!string.IsNullOrWhiteSpace(fieldsOverride))
            {
                listRequest.Fields = fieldsOverride.Trim();
            }

            // Recursively get files from drive
            var files = await GetFilesInDrive(listRequest, null).ConfigureAwait(false);
            LoggingHandler.LogInfo($"GoogleClientService: GetFilesInDriveAsync: folderId: {driveId}");
            LoggingHandler.LogInfo($"GoogleClientService: GetFilesInDriveAsync: files.Count: {files.Count}");
            return files;
        }

        public async Task<File> GetFileByNameInDriveAsync(string driveId, string fileName)
        {
            var files = await GetFilesInDriveAsync(driveId).ConfigureAwait(false);

            return files.Where(f => f.Name == fileName).FirstOrDefault();
        }

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

        public Task CopyFileInDrive(string fileId, string destinationFolderId, string fileName)
        {
            var newFile = new File()
            {
                Name = fileName,
                Parents = new List<string> { destinationFolderId }
            };

            var updateRequest = _driveService.Files.Copy(newFile, fileId);
            var renamedFile = updateRequest.Execute();
            LoggingHandler.LogInfo($"File {fileName} copied to {destinationFolderId} - New ID: {renamedFile.Id}");
            return Task.CompletedTask;
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

                LoggingHandler.LogInfo($"Upload progress: {JsonConvert.SerializeObject(createdFile)}");

                return createdFile.Status == UploadStatus.Completed;
            }
            catch (Exception exc)
            {
                LoggingHandler.LogError($"Error uploading file");
                LoggingHandler.LogError($"Upload progress: {JsonConvert.SerializeObject(createdFile)}");
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

                LoggingHandler.LogInfo($"Upload progress: {JsonConvert.SerializeObject(createdFile)}");

                return createdFile.Status == UploadStatus.Completed;
            }
            catch (Exception exc)
            {
                LoggingHandler.LogError($"Error uploading csv file");
                LoggingHandler.LogError($"Upload progress: {JsonConvert.SerializeObject(createdFile)}");
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


        #region Clean Google Drive code

        private CreateMediaUpload GetCreateFileOnDriveRequest(FileInMemory fileInMemory, string targetFolderId)
        {
            var newFileDescription = new FileDescription()
            {
                Name = fileInMemory.Name,
                MimeType = fileInMemory.MimeType,
                Parents = new List<string> { targetFolderId }
            };

            var createFileRequest = _driveService.Files
                .Create(newFileDescription, fileInMemory.DataStream, fileInMemory.MimeType);

            return createFileRequest;
        }

        public async Task<IUploadProgress> UploadFileToDrive(FileInMemory fileInMemory, string targetFolderId)
        {
            var createFileRequest = GetCreateFileOnDriveRequest(fileInMemory, targetFolderId);
            var uploadProgress = await createFileRequest.UploadAsync().ConfigureAwait(false);
            return uploadProgress;
        }

        public async Task UploadFileOrThrow(FileInMemory fileInMemory, string targetFolderId)
        {
            var uploadStateWrapper = await UploadFileToDrive(fileInMemory, targetFolderId).ConfigureAwait(false);

            if (uploadStateWrapper.Status == UploadStatus.Failed)
            {
                throw uploadStateWrapper.Exception;
            }
        }

        #endregion
    }
}
