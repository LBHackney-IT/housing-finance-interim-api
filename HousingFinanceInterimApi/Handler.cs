using Amazon.Lambda.Core;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Gateways.Options;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HousingFinanceInterimApi
{

    public class Handler
    {

        private readonly IGoogleClientService _googleClientService;
        private readonly IUPCashFileNameGateway _cashFileNameGateway;
        private readonly IUPCashDumpGateway _cashDumpGateway;

        public Handler()
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);
            DatabaseContext context = new DatabaseContext(optionsBuilder.Options);

            var options = Options.Create(new GoogleClientServiceOptions
            {
                ApplicationName = "",
                Scopes = new List<string>
                {
                    // TODO
                    "", ""
                }
            });
            IGoogleClientServiceFactory serviceFactory = new GoogleClientServiceFactory(default, options, context);

            _googleClientService =
                serviceFactory.CreateGoogleClientServiceForApiKey(Environment.GetEnvironmentVariable("GOOGLE_API_KEY"));
        }

        public async Task ImportFiles()
        {
            // TODO drive ID
            var folderFiles = await _googleClientService.GetFilesInDriveAsync("").ConfigureAwait(false);

            foreach (var folderFile in folderFiles.Where(item => item.Name.EndsWith(".dat")))
            {
                try
                {
                    // Check if entry already made
                    var getResult = await _cashFileNameGateway.GetAsync(folderFile.Name).ConfigureAwait(false);

                    if (getResult == null)
                    {
                        // Create file entry
                        var createResult = await _cashFileNameGateway.CreateAsync(folderFile.Name).ConfigureAwait(false);

                        if (createResult != null)
                        {
                            IList<string> fileLines = await _googleClientService
                                .ReadFileLineDataAsync(folderFile.Name, folderFile.Id, folderFile.MimeType)
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
                                    var result = await _cashDumpGateway.CreateBulkAsync(createResult.Id, batch)
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
