using Amazon.Lambda.Core;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Gateways.Options;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Sheets.v4;
using HousingFinanceInterimApi.V1.Domain;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HousingFinanceInterimApi
{

    /// <summary>
    /// The Lambda scheduled job handler.
    /// </summary>
    public class Handler
    {

        private readonly IGoogleClientService _googleClientService;
        private readonly IUPCashFileNameGateway _cashFileNameGateway;
        private readonly IUPCashDumpGateway _cashDumpGateway;

        /// <summary>
        /// The google file settings list use case
        /// </summary>
        private readonly IListGoogleFileSettingsUseCase _googleFileSettingsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="Handler"/> class.
        /// </summary>
        public Handler()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
            DatabaseContext context = new DatabaseContext(optionsBuilder.Options);

            // Create the file settings gateway
            IGoogleFileSettingGateway settingGateway = new GoogleFileSettingGateway(context);

            // Create a list Google file settings use case
            _googleFileSettingsList = new ListGoogleFileSettingsUseCase(settingGateway);

            // Create a google client service factory and instance
            IOptions<GoogleClientServiceOptions> options = Options.Create(new GoogleClientServiceOptions
            {
                ApplicationName = "Hackney Finance Interim Solution",
                Scopes = new List<string>
                {
                    DriveService.Scope.DriveReadonly, SheetsService.Scope.SpreadsheetsReadonly
                }
            });
            IGoogleClientServiceFactory serviceFactory = new GoogleClientServiceFactory(default, options, context);

            _googleClientService =
                serviceFactory.CreateGoogleClientServiceForApiKey(Environment.GetEnvironmentVariable("GOOGLE_API_KEY"));
        }

        /// <summary>
        /// Imports the files.
        /// </summary>
        public async Task ImportFiles()
        {
            IList<GoogleFileSettingDomain> googleFileSettings =
                await _googleFileSettingsList.Execute().ConfigureAwait(false);

            // Sequentially execute, for parallel execution, each will need a database context
            foreach (GoogleFileSettingDomain googleFileSettingItem in googleFileSettings)
            {
                // Retrieve files from this folder
                IList<File> folderFiles = await _googleClientService
                    .GetFilesInDriveAsync(googleFileSettingItem.GoogleFolderId)
                    .ConfigureAwait(false);

                // If we have folder files
                if (folderFiles.Any())
                {
                    // Filter to file types
                    folderFiles = folderFiles.Where(item => item.FileExtension.Equals(googleFileSettingItem.FileType))
                        .ToList();

                    const string G_SHEET = ".gsheet";
                    const string DAT_FILE = ".dat";

                    switch (googleFileSettingItem.FileType)
                    {
                        case G_SHEET:
                        {
                            break;
                        }
                        case DAT_FILE:
                        {
                            await HandleDatFileDownloads(folderFiles).ConfigureAwait(false);

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the dat file downloads.
        /// </summary>
        /// <param name="files">The files to download from.</param>
        private async Task HandleDatFileDownloads(IEnumerable<File> files)
        {
            foreach (File fileItem in files)
            {
                try
                {
                    // Check if entry already made
                    UPCashDumpFileName getResult =
                        await _cashFileNameGateway.GetAsync(fileItem.Name).ConfigureAwait(false);

                    if (getResult == null)
                    {
                        // Create file entry
                        UPCashDumpFileName createResult =
                            await _cashFileNameGateway.CreateAsync(fileItem.Name).ConfigureAwait(false);

                        if (createResult != null)
                        {
                            IList<string> fileLines = await _googleClientService
                                .ReadFileLineDataAsync(fileItem.Name, fileItem.Id, fileItem.MimeType)
                                .ConfigureAwait(false);

                            // Ensure no blank lines
                            fileLines = fileLines.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();

                            const int TAKE = 1000;
                            int skip = 0;
                            bool failure = false;
                            IList<string> batch;

                            do
                            {
                                // Create a batch
                                batch = fileLines.Skip(skip).Take(TAKE).ToList();

                                if (batch.Any())
                                {
                                    // Bulk insert the lines
                                    IList<UPCashDump> result = await _cashDumpGateway
                                        .CreateBulkAsync(createResult.Id, batch)
                                        .ConfigureAwait(false);

                                    // Determine failure
                                    bool batchFailure = result == null;

                                    if (batchFailure)
                                    {
                                        failure = true;
                                    }

                                    Console.WriteLine(batchFailure
                                        ? $"File failure: {createResult.Id}"
                                        : $"File lines created {result.Count} for file {createResult.Id}");
                                    skip += TAKE;
                                }
                            }
                            while (batch.Any());

                            // If success, set the status
                            if (!failure)
                            {
                                Console.WriteLine("File success");
                                await _cashFileNameGateway.SetToSuccessAsync(createResult.Id).ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    // TODO error log
                    Console.WriteLine(exc);

                    throw;
                }
            }
        }

    }

}
