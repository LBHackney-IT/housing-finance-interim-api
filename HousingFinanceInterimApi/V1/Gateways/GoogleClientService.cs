using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
            IDownloadProgress progress = await request.DownloadAsync(stream).ConfigureAwait(true);

            if (progress.Status == DownloadStatus.Completed)
            {
                if (!Directory.Exists("tempfiles"))
                {
                    Directory.CreateDirectory("tempfiles");
                }

                string outputPath = $"tempfiles/{fileName}";

                await using (FileStream file = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    stream.WriteTo(file);
                }

                if (File.Exists(outputPath))
                {
                    results = await File.ReadAllLinesAsync(outputPath).ConfigureAwait(true);
                    File.Delete(outputPath);
                }

                return results;
            }
            else
            {
                // TODO log
            }

            return results;
        }

        #endregion

        #region Google Sheets

        public Task ReadSheetToJsonAsync(string spreadSheetId, string sheetName, string sheetRange, string outputFileName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

    }

}
