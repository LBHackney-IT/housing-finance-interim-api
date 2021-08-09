using Amazon.Lambda.Core;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Sheets.v4;
using HousingFinanceInterimApi.V1.Domain;
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
using AutoMapper;
using Google.Apis.Logging;
using HousingFinanceInterimApi.V1.Domain.AutoMaps;
using Mapster;
using Microsoft.Extensions.Logging;
using ILogger = Google.Apis.Logging.ILogger;
using LogLevel = Google.Apis.Logging.LogLevel;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HousingFinanceInterimApi
{

    /// <summary>
    /// The Lambda scheduled job handler.
    /// </summary>
    public class Handler
    {

        /// <summary>
        /// The log error use case
        /// </summary>
        private readonly ILogErrorUseCase _logErrorUseCase;

        /// <summary>
        /// The get files in google drive use case
        /// </summary>
        private readonly IGetFilesInGoogleDriveUseCase _getFilesInGoogleDriveUseCase;

        /// <summary>
        /// The read google file line data use case
        /// </summary>
        private readonly IReadGoogleFileLineDataUseCase _readGoogleFileLineDataUseCase;

        /// <summary>
        /// The read google sheet to entities use case
        /// </summary>
        private readonly IReadGoogleSheetToEntities _readGoogleSheetToEntitiesUseCase;

        /// <summary>
        /// The save rent breakdowns use case
        /// </summary>
        private readonly ISaveRentBreakdownsUseCase _saveRentBreakdownsUseCase;

        /// <summary>
        /// The save current rent positions use case
        /// </summary>
        private readonly ISaveCurrentRentPositionsUseCase _saveCurrentRentPositionsUseCase;

        /// <summary>
        /// The save service charges payments received use case
        /// </summary>
        private readonly ISaveServiceChargePaymentsReceivedUseCase _saveServiceChargePaymentsReceivedUseCase;

        /// <summary>
        /// The save leasehold accounts use case
        /// </summary>
        private readonly ISaveLeaseholdAccountsUseCase _saveLeaseholdAccountsUseCase;

        /// <summary>
        /// The save other HRA use case
        /// </summary>
        private readonly ISaveOtherHRAUseCase _saveOtherHRAUseCase;

        /// <summary>
        /// The save garages use case
        /// </summary>
        private readonly ISaveGaragesUseCase _saveGaragesUseCase;

        /// <summary>
        /// The refresh manage arrears use case
        /// </summary>
        private readonly IRefreshManageArrearsUseCase _refreshManageArrearsUseCase;

        /// <summary>
        /// The create bulk cash dumps use case
        /// </summary>
        private readonly ICreateBulkCashDumpsUseCase _createBulkCashDumpsUseCase;

        /// <summary>
        /// The get up cash file name use case
        /// </summary>
        private readonly IGetUPCashFileNameUseCase _getUpCashFileNameUseCase;

        /// <summary>
        /// The create up cash file name use case
        /// </summary>
        private readonly ICreateUPCashFileNameUseCase _createUpCashFileNameUseCase;

        /// <summary>
        /// The set up cash file name success use case
        /// </summary>
        private readonly ISetUPCashFileNameSuccessUseCase _setUpCashFileNameSuccessUseCase;

        /// <summary>
        /// The create bulk housing cash dumps use case
        /// </summary>
        private readonly ICreateBulkHousingCashDumpsUseCase _createBulkHousingCashDumpsUseCase;

        /// <summary>
        /// The get up housing cash file name use case
        /// </summary>
        private readonly IGetUPHousingCashFileNameUseCase _getUpHousingCashFileNameUseCase;

        /// <summary>
        /// The create up housing cash file name use case
        /// </summary>
        private readonly ICreateUPHousingCashFileNameUseCase _createUpHousingCashFileNameUseCase;

        /// <summary>
        /// The set up housing cash file name success use case
        /// </summary>
        private readonly ISetUPHousingCashFileNameSuccessUseCase _setUpHousingCashFileNameSuccessUseCase;

        private readonly ILoadTransactionsUseCase _loadTransactionsUseCase;

        /// <summary>
        /// The google file settings list use case
        /// </summary>
        private readonly IListGoogleFileSettingsUseCase _googleFileSettingsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="Handler"/> class.
        /// </summary>
        public Handler()
        {
            // Create auto mapper instance
            var mapperConfig = new MapperConfiguration(mapperConfiguration =>
            {
                mapperConfiguration.AddProfile(new RentBreakdownMappingProfile());
                mapperConfiguration.AddProfile(new CurrentRentPositionMappingProfile());
                mapperConfiguration.AddProfile(new ServiceChargePaymentsReceivedMappingProfile());
                mapperConfiguration.AddProfile(new LeaseholdAccountMappingProfile());
                mapperConfiguration.AddProfile(new GarageMappingProfile());
                mapperConfiguration.AddProfile(new OtherHRAMappingProfile());
            });
            IMapper autoMapper = mapperConfig.CreateMapper();

            // Create database context
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

            DatabaseContext context = new DatabaseContext(optionsBuilder.Options);

            // Error log use cases
            IErrorLogGateway errorLogGateway = new ErrorLogGateway(context);
            _logErrorUseCase = new LogErrorUseCase(errorLogGateway);

            // Rent breakdown use cases
            IRentBreakdownGateway rentBreakdownGateway = new RentBreakdownGateway(context);
            _saveRentBreakdownsUseCase = new SaveRentBreakdownsUseCase(autoMapper, rentBreakdownGateway);

            // Current rent position use cases
            ICurrentRentPositionGateway currentRentPositionGateway = new CurrentRentPositionGateway(context);

            _saveCurrentRentPositionsUseCase =
                new SaveCurrentRentPositionsUseCase(autoMapper, currentRentPositionGateway);

            // Service charges payments received use cases
            IServiceChargePaymentsReceivedGateway serviceChargePaymentsReceivedGateway = new ServiceChargePaymentsReceivedGateway(context);
            _saveServiceChargePaymentsReceivedUseCase = new SaveServiceChargePaymentsReceivedUseCase(autoMapper, serviceChargePaymentsReceivedGateway);

            // Leasehold accounts use cases
            ILeaseholdAccountsGateway leaseholdAccountsGateway = new LeaseholdAccountsGateway(context);
            _saveLeaseholdAccountsUseCase = new SaveLeaseholdAccountsUseCase(autoMapper, leaseholdAccountsGateway);

            // Garage use cases
            IGarageGateway garageGateway = new GarageGateway(context);
            _saveGaragesUseCase = new SaveGaragesUseCase(autoMapper, garageGateway);

            // Other HRA use cases
            IOtherHRAGateway otherHraGateway = new OtherHRAGateway(context);
            _saveOtherHRAUseCase = new SaveOtherHRAUseCase(autoMapper, otherHraGateway);

            // Other HRA use cases
            IRefreshManageArrearsGateway refreshManageArrearsGateway = new RefreshManageArrearsGateway(context);
            _refreshManageArrearsUseCase = new RefreshManageArrearsUseCase(refreshManageArrearsGateway);

            // File name use cases
            IUPCashFileNameGateway fileNameGateway = new UPCashFileNameGateway(context);
            _getUpCashFileNameUseCase = new GetUPCashFileNameUseCase(fileNameGateway);
            _createUpCashFileNameUseCase = new CreateUPCashFileNameUseCase(fileNameGateway);
            _setUpCashFileNameSuccessUseCase = new SetUPCashFileNameSuccessUseCase(fileNameGateway);

            // Cash dump use cases
            IUPCashDumpGateway cashDumpGateway = new UPCashDumpGateway(context);
            _createBulkCashDumpsUseCase = new CreateBulkCashDumpsUseCase(cashDumpGateway);

            // Housing File name use cases
            IUPHousingCashFileNameGateway housingFileNameGateway = new UPHousingCashFileNameGateway(context);
            _getUpHousingCashFileNameUseCase = new GetUPHousingCashFileNameUseCase(housingFileNameGateway);
            _createUpHousingCashFileNameUseCase = new CreateUPHousingCashFileNameUseCase(housingFileNameGateway);
            _setUpHousingCashFileNameSuccessUseCase = new SetUPHousingCashFileNameSuccessUseCase(housingFileNameGateway);

            // Housing cash dump use cases
            IUPHousingCashDumpGateway housingCashDumpGateway = new UPHousingCashDumpGateway(context);
            _createBulkHousingCashDumpsUseCase = new CreateBulkHousingCashDumpsUseCase(housingCashDumpGateway);

            // Google file setting use cases
            IGoogleFileSettingGateway settingGateway = new GoogleFileSettingGateway(context);
            _googleFileSettingsList = new ListGoogleFileSettingsUseCase(settingGateway);

            ITransactionGateway transactionGateway = new TransactionGateway(context);
            IUPCashLoadGateway upCashLoadGateway = new UPCashLoadGateway(context);
            _loadTransactionsUseCase = new LoadTransactionsUseCase(transactionGateway, upCashLoadGateway);

            // Create a google client service factory and instance
            IOptions<GoogleClientServiceOptions> options = Options.Create(new GoogleClientServiceOptions
            {
                ApplicationName = "Hackney Finance Interim Solution",
                Scopes = new List<string>
                {
                    DriveService.Scope.DriveReadonly, SheetsService.Scope.SpreadsheetsReadonly
                }
            });

            // Google service use cases
            IGoogleClientService googleClientService =
                new GoogleClientServiceFactory(default, options, context)
                    .CreateGoogleClientServiceFromJson(Environment.GetEnvironmentVariable("GOOGLE_API_KEY"));

            _getFilesInGoogleDriveUseCase = new GetFilesInGoogleDriveUseCase(googleClientService);
            _readGoogleFileLineDataUseCase = new ReadGoogleFileLineDataUseCase(googleClientService);
            _readGoogleSheetToEntitiesUseCase = new ReadGoogleSheetToEntities(googleClientService);
        }

        /// <summary>
        /// Imports the cash files.
        /// </summary>
        public async Task ImportFiles()
        {
            const string FILE_LABEL = "Cash Files";

            IList<GoogleFileSettingDomain> googleFileSettings =
                (await _googleFileSettingsList.Execute().ConfigureAwait(false)).Where(item
                    => item.Label.Equals(FILE_LABEL, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
            Console.WriteLine($"{googleFileSettings.Count} google dat file settings found");

            // Sequentially execute, for parallel execution, each will need a database context
            foreach (GoogleFileSettingDomain googleFileSettingItem in googleFileSettings)
            {
                // Retrieve files from this folder
                IList<File> folderFiles = await _getFilesInGoogleDriveUseCase
                    .ExecuteAsync(googleFileSettingItem.GoogleIdentifier)
                    .ConfigureAwait(false);

                // If we have folder files
                if (folderFiles.Any())
                {
                    // Filter to file types
                    folderFiles = folderFiles.Where(item => item.Name.EndsWith(googleFileSettingItem.FileType))
                        .ToList();

                    await HandleDatFileDownloads(folderFiles).ConfigureAwait(false);
                }
            }
        }

        public async Task LoadCashFileTransactions()
        {
            var transaction = await _loadTransactionsUseCase.LoadCashFilesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Imports the housing benefit files.
        /// </summary>
        public async Task ImportHousingFiles()
        {
            const string FILE_LABEL = "Housing Cash Files";

            IList<GoogleFileSettingDomain> googleFileSettings =
                (await _googleFileSettingsList.Execute().ConfigureAwait(false)).Where(item
                    => item.Label.Equals(FILE_LABEL, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
            Console.WriteLine($"{googleFileSettings.Count} google dat file settings found");



            // Sequentially execute, for parallel execution, each will need a database context
            foreach (GoogleFileSettingDomain googleFileSettingItem in googleFileSettings)
            {
                // Retrieve files from this folder
                IList<File> folderFiles = await _getFilesInGoogleDriveUseCase
                    .ExecuteAsync(googleFileSettingItem.GoogleIdentifier)
                    .ConfigureAwait(false);

                foreach (var folder in folderFiles.Where(f => f.MimeType.Equals("application/vnd.google-apps.folder")).ToList())
                {
                    var files = await _getFilesInGoogleDriveUseCase.ExecuteAsync(folder.Id);
                    foreach (var file in files)
                        folderFiles.Add(file);
                }

                // If we have folder files
                if (folderFiles.Any())
                {
                    // Filter to file types
                    folderFiles = folderFiles.Where(item => item.Name.StartsWith("rentpost") && item.Name.EndsWith(googleFileSettingItem.FileType))
                        .ToList();

                    await HandleDatHousingFileDownloads(folderFiles).ConfigureAwait(false);
                }
            }
        }

        public async Task LoadHousingFileTransactions()
        {
            var transaction = await _loadTransactionsUseCase.LoadHousingFilesAsync().ConfigureAwait(false);
        }


        /// <summary>
        /// Imports the rent breakdowns.
        /// </summary>
        /// <exception cref="Exception">
        /// Failed to save rent breakdown items
        /// or
        /// No Google file setting found for {nameof(ImportRentBreakdowns)}
        /// </exception>
        public async Task ImportRentBreakdowns()
        {
            GoogleFileSettingDomain googleFileSetting =
                (await _googleFileSettingsList.Execute().ConfigureAwait(false)).First(item
                    => item.Label.Equals("Rent Breakdown", StringComparison.CurrentCultureIgnoreCase));

            if (googleFileSetting != null)
            {
                IList<RentBreakdownDomain> data = await _readGoogleSheetToEntitiesUseCase
                    .ExecuteAsync<RentBreakdownDomain>(googleFileSetting.GoogleIdentifier, "Rent Debit By Account",
                        "A:AU")
                    .ConfigureAwait(false);

                // Save data
                var saveResult = await _saveRentBreakdownsUseCase.ExecuteAsync(data).ConfigureAwait(false);

                if (saveResult <= 0)
                {
                    throw new Exception("Failed to save rent breakdown items");
                }
            }
            else
            {
                throw new Exception($"No Google file setting found for {nameof(ImportRentBreakdowns)}");
            }
        }

        /// <summary>
        /// Imports the rent positions.
        /// </summary>
        /// <exception cref="Exception">
        /// Failed to save rent position items
        /// or
        /// No Google file setting found for {nameof(ImportRentPositions)}
        /// </exception>
        public async Task ImportRentPositions()
        {
            GoogleFileSettingDomain googleFileSetting =
                (await _googleFileSettingsList.Execute().ConfigureAwait(false)).First(item
                    => item.Label.Equals("Rent Position", StringComparison.CurrentCultureIgnoreCase));

            if (googleFileSetting != null)
            {
                IList<CurrentRentPositionDomain> data = await _readGoogleSheetToEntitiesUseCase
                    .ExecuteAsync<CurrentRentPositionDomain>(googleFileSetting.GoogleIdentifier, "Weekly Payments",
                        "A:BB")
                    .ConfigureAwait(false);

                // Save data
                var saveResult = await _saveCurrentRentPositionsUseCase.ExecuteAsync(data).ConfigureAwait(false);

                if (saveResult <= 0)
                {
                    throw new Exception("Failed to save rent position items");
                }
            }
            else
            {
                throw new Exception($"No Google file setting found for {nameof(ImportRentPositions)}");
            }
        }

        /// <summary>
        /// Imports the rent positions.
        /// </summary>
        /// <exception cref="Exception">
        /// Failed to save service charge payments received items
        /// or
        /// No Google file setting found for {nameof(ImportServiceChargePaymentsReceived)}
        /// </exception>
        public async Task ImportServiceChargePaymentsReceived()
        {
            GoogleFileSettingDomain googleFileSetting =
                (await _googleFileSettingsList.Execute().ConfigureAwait(false)).First(item
                    => item.Label.Equals("Service Charge Payments Received", StringComparison.CurrentCultureIgnoreCase));

            if (googleFileSetting != null)
            {
                IList<ServiceChargePaymentsReceivedDomain> data = await _readGoogleSheetToEntitiesUseCase
                    .ExecuteAsync<ServiceChargePaymentsReceivedDomain>(googleFileSetting.GoogleIdentifier, "Monthly SC Payments",
                        "A:AB")
                    .ConfigureAwait(false);

                // Save data
                var saveResult = await _saveServiceChargePaymentsReceivedUseCase.ExecuteAsync(data).ConfigureAwait(false);

                if (saveResult <= 0)
                {
                    throw new Exception("Failed to save service charges payments received items");
                }
            }
            else
            {
                throw new Exception($"No Google file setting found for {nameof(ImportServiceChargePaymentsReceived)}");
            }
        }

        /// <summary>
        /// Imports the leasehold accounts.
        /// </summary>
        /// <exception cref="Exception">
        /// Failed to save leasehold accounts items
        /// or
        /// No Google file setting found for {nameof(ImportLeaseholdAccounts)}
        /// </exception>
        public async Task ImportLeaseholdAccounts()
        {
            GoogleFileSettingDomain googleFileSetting =
                (await _googleFileSettingsList.Execute().ConfigureAwait(false)).First(item
                    => item.Label.Equals("Leasehold Accounts", StringComparison.CurrentCultureIgnoreCase));

            if (googleFileSetting != null)
            {
                IList<LeaseholdAccountDomain> data = await _readGoogleSheetToEntitiesUseCase
                    .ExecuteAsync<LeaseholdAccountDomain>(googleFileSetting.GoogleIdentifier, "Current",
                        "A:M")
                    .ConfigureAwait(false);

                // Save data
                var saveResult = await _saveLeaseholdAccountsUseCase.ExecuteAsync(data).ConfigureAwait(false);

                if (saveResult <= 0)
                {
                    throw new Exception("Failed to save leasehold accounts items");
                }
            }
            else
            {
                throw new Exception($"No Google file setting found for {nameof(ImportLeaseholdAccounts)}");
            }
        }

        /// <summary>
        /// Imports temp accomm and garage charges.
        /// </summary>
        /// <exception cref="Exception">
        /// Failed to save tem accomm and garages
        /// or
        /// No Google file setting found for {nameof(ImportTemporaryAccommodation)}
        /// </exception>
        public async Task ImportTemporaryAccommodation()
        {
            GoogleFileSettingDomain googleFileSetting =
                (await _googleFileSettingsList.Execute().ConfigureAwait(false)).First(item
                    => item.Label.Equals("Temp Acc And Garages", StringComparison.CurrentCultureIgnoreCase));

            if (googleFileSetting != null)
            {
                IList<OtherHRADomain> data = await _readGoogleSheetToEntitiesUseCase
                    .ExecuteAsync<OtherHRADomain>(googleFileSetting.GoogleIdentifier, "Sheet1",
                        "A:BC")
                    .ConfigureAwait(false);

                // Save data
                var saveResult = await _saveOtherHRAUseCase.ExecuteAsync(data).ConfigureAwait(false);

                if (saveResult <= 0)
                {
                    throw new Exception("Failed to save leasehold accounts items");
                }
            }
            else
            {
                throw new Exception($"No Google file setting found for {nameof(ImportLeaseholdAccounts)}");
            }
        }

        /// <summary>
        /// Imports the rent breakdowns.
        /// </summary>
        /// <exception cref="Exception">
        /// Failed to save rent breakdown items
        /// or
        /// No Google file setting found for {nameof(ImportRentBreakdowns)}
        /// </exception>
        public async Task ImportGarage()
        {
            GoogleFileSettingDomain googleFileSetting =
                (await _googleFileSettingsList.Execute().ConfigureAwait(false)).First(item
                    => item.Label.Equals("Garage Database", StringComparison.CurrentCultureIgnoreCase));

            if (googleFileSetting != null)
            {
                IList<GarageDomain> data = await _readGoogleSheetToEntitiesUseCase
                    .ExecuteAsync<GarageDomain>(googleFileSetting.GoogleIdentifier, "Sheet1",
                        "A:AC")
                    .ConfigureAwait(false);

                // Save data
                var saveResult = await _saveGaragesUseCase.ExecuteAsync(data).ConfigureAwait(false);

                if (saveResult <= 0)
                {
                    throw new Exception("Failed to save garage items");
                }
            }
            else
            {
                throw new Exception($"No Google file setting found for {nameof(ImportGarage)}");
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
                    UPCashFileNameDomain getResult =
                        await _getUpCashFileNameUseCase.ExecuteAsync(fileItem.Name).ConfigureAwait(false);

                    if (getResult == null)
                    {
                        // Create file entry
                        UPCashFileNameDomain createResult = await _createUpCashFileNameUseCase.ExecuteAsync(fileItem.Name)
                            .ConfigureAwait(false);

                        if (createResult != null)
                        {
                            IList<string> fileLines = await _readGoogleFileLineDataUseCase
                                .ExecuteAsync(fileItem.Name, fileItem.Id, fileItem.MimeType)
                                .ConfigureAwait(false);

                            // Ensure no blank lines
                            fileLines = fileLines.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();

                            bool failure = false;
                            IList<UPCashDumpDomain> result = await _createBulkCashDumpsUseCase
                                .ExecuteAsync(createResult.Id, fileLines)
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

                            // If success, set the status
                            if (!failure)
                            {
                                Console.WriteLine("File success");

                                await _setUpCashFileNameSuccessUseCase.ExecuteAsync(createResult.Id)
                                    .ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    string namespaceLabel =
                        $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleDatFileDownloads)}";

                    // Log error
                    await _logErrorUseCase
                        .ExecuteAsync($"{nameof(UPCashDumpFileName)} -- {nameof(UPCashDump)} -- {fileItem.Name}", null,
                            $"{namespaceLabel} application error", exc.ToString())
                        .ConfigureAwait(false);

                    throw;
                }
            }
        }

        /// <summary>
        /// Handles the dat housing file downloads.
        /// </summary>
        /// <param name="files">The files to download from.</param>
        private async Task HandleDatHousingFileDownloads(IEnumerable<File> files)
        {
            foreach (File fileItem in files)
            {
                try
                {
                    // Check if entry already made
                    UPHousingCashFileNameDomain getResult =
                        await _getUpHousingCashFileNameUseCase.ExecuteAsync(fileItem.Name).ConfigureAwait(false);

                    if (getResult == null)
                    {
                        // Create file entry
                        UPHousingCashFileNameDomain createResult = await _createUpHousingCashFileNameUseCase.ExecuteAsync(fileItem.Name)
                            .ConfigureAwait(false);

                        if (createResult != null)
                        {
                            IList<string> fileLines = await _readGoogleFileLineDataUseCase
                                .ExecuteAsync(fileItem.Name, fileItem.Id, fileItem.MimeType)
                                .ConfigureAwait(false);

                            // Ensure no blank lines
                            fileLines = fileLines.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();

                            bool failure = false;
                            IList<UPHousingCashDumpDomain> result = await _createBulkHousingCashDumpsUseCase
                                .ExecuteAsync(createResult.Id, fileLines)
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

                            //const int TAKE = 250;
                            //int skip = 0;
                            //bool failure = false;
                            //IList<string> batch;

                            //do
                            //{
                            //    // Create a batch
                            //    batch = fileLines.Skip(skip).Take(TAKE).ToList();

                            //    if (batch.Any())
                            //    {
                            //        // Bulk insert the lines
                            //        IList<UPHousingCashDumpDomain> result = await _createBulkHousingCashDumpsUseCase
                            //            .ExecuteAsync(createResult.Id, batch)
                            //            .ConfigureAwait(false);

                            //        // Determine failure
                            //        bool batchFailure = result == null;

                            //        if (batchFailure)
                            //        {
                            //            failure = true;
                            //        }

                            //        Console.WriteLine(batchFailure
                            //            ? $"File failure: {createResult.Id}"
                            //            : $"File lines created {result.Count} for file {createResult.Id}");
                            //        skip += TAKE;
                            //    }
                            //}
                            //while (batch.Any());

                            // If success, set the status
                            if (!failure)
                            {
                                Console.WriteLine("File success");

                                await _setUpHousingCashFileNameSuccessUseCase.ExecuteAsync(createResult.Id)
                                    .ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    string namespaceLabel =
                        $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleDatHousingFileDownloads)}";

                    // Log error
                    await _logErrorUseCase
                        .ExecuteAsync($"{nameof(UPHousingCashDumpFileName)} -- {nameof(UPHousingCashDump)} -- {fileItem.Name}", null,
                            $"{namespaceLabel} application error", exc.ToString())
                        .ConfigureAwait(false);

                    throw;
                }
            }
        }

        /// <summary>
        /// Imports the rent breakdowns.
        /// </summary>
        /// <exception cref="Exception">
        /// Failed to save rent breakdown items
        /// or
        /// No Google file setting found for {nameof(ImportRentBreakdowns)}
        /// </exception>
        public async Task RefreshManageArrearsTable()
        {
            try
            {
                await _refreshManageArrearsUseCase.ExecuteAsync().ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                throw new Exception("Failed to refresh manage arrears tables");
            }
        }

    }

}
