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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Google.Apis.Logging;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain.AutoMaps;
using HousingFinanceInterimApi.V1.Handlers;
using Mapster;
using Microsoft.Extensions.Http.Logging;
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
        private readonly ICreateBatchLogUseCase _createBatchLogUseCase;

        private readonly ISetBatchLogSuccessUseCase _setBatchLogSuccessUseCase;

        private readonly ICreateBatchLogErrorUseCase _createBatchLogErrorUseCase;

        private readonly IRenameGoogleFileUseCase _renameGoogleFileUseCase;

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

        //private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private readonly string _waitDuration = "30";

        private readonly int _batchSize = Convert.ToInt32("250");
        private readonly string _cashFileRegex = @"^CashFile\d{4}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01]).dat$";
        private readonly string _housingBenefitFileRegex = @"^HousingBenefitFile\d{4}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])$";

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


            IBatchLogGateway batchLogGateway = new BatchLogGateway(context);
            IBatchLogErrorGateway batchLogErrorGateway = new BatchLogErrorGateway(context);

            _createBatchLogUseCase = new CreateBatchLogUseCase(batchLogGateway);
            _createBatchLogErrorUseCase = new CreateBatchLogErrorUseCase(batchLogErrorGateway);
            _setBatchLogSuccessUseCase = new SetBatchLogSuccessUseCase(batchLogGateway);





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
            IUPHousingCashLoadGateway upHousingCashLoadGateway = new UPHousingCashLoadGateway(context);
            _loadTransactionsUseCase = new LoadTransactionsUseCase(transactionGateway, upCashLoadGateway, upHousingCashLoadGateway);

            // Create a google client service factory and instance
            IOptions<GoogleClientServiceOptions> options = Options.Create(new GoogleClientServiceOptions
            {
                ApplicationName = "Hackney Finance Interim Solution",
                Scopes = new List<string>
                {
                    DriveService.Scope.Drive, SheetsService.Scope.SpreadsheetsReadonly
                }
            });

            //Google service use cases
            IGoogleClientService googleClientService =
                new GoogleClientServiceFactory(default, options, context)
                    .CreateGoogleClientServiceFromJson(Environment.GetEnvironmentVariable("GOOGLE_API_KEY"));

            _getFilesInGoogleDriveUseCase = new GetFilesInGoogleDriveUseCase(googleClientService);
            _readGoogleFileLineDataUseCase = new ReadGoogleFileLineDataUseCase(googleClientService);
            _readGoogleSheetToEntitiesUseCase = new ReadGoogleSheetToEntities(googleClientService);
            _renameGoogleFileUseCase = new RenameGoogleFileUseCase(googleClientService);
        }

        public async Task<CheckFileResponse> CheckCashFiles()
        {
            LoggingHandler.LogInfo($"CHECKING IF EXIST PENDING CASH FILES");
            string label = "CashFile";
            var existFile = await CheckExistFiles(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"EXIST PENDING CASH FILES: {existFile}");

            return new CheckFileResponse()
            {
                ExistFile = existFile,
                NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };
        }

        public async Task<CheckFileResponse> CheckHousingBenefitFiles()
        {
            LoggingHandler.LogInfo($"CHECKING IF EXISTS PENDING HOUSING BENEFIT FILES");
            string label = "HousingBenefitFiles";
            var existFile = await CheckExistFiles(label).ConfigureAwait(false);
            LoggingHandler.LogInfo($"EXIST PENDING HOUSING BENEFIT FILES: {existFile}");

            return new CheckFileResponse()
            {
                ExistFile = existFile,
                NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };
        }

        private async Task<IList<GoogleFileSettingDomain>> GetGoogleFileSetting(string label)
        {
            LoggingHandler.LogInfo($"GETTING GOOGLE FILE SETTINGS FOR '{label}' LABEL");

            IList<GoogleFileSettingDomain> googleFileSettings =
                (await _googleFileSettingsList.Execute().ConfigureAwait(false)).Where(item
                    => item.Label.Equals(label, StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            LoggingHandler.LogInfo($"{googleFileSettings.Count} GOOGLE FILE SETTINGS FOUND");

            return googleFileSettings;
        }

        private async Task<bool> CheckExistFiles(string label)
        {
            var listNotStart = new List<string>();
            listNotStart.Add("OK_");
            listNotStart.Add("NOK_");

            var googleFileSettings = await GetGoogleFileSetting(label).ConfigureAwait(false);

            foreach (GoogleFileSettingDomain googleFileSettingItem in googleFileSettings)
            {
                IList<File> folderFiles = await _getFilesInGoogleDriveUseCase
                    .ExecuteAsync(googleFileSettingItem.GoogleIdentifier)
                    .ConfigureAwait(false);

                folderFiles = folderFiles.Where(item => item.Name.EndsWith(googleFileSettingItem.FileType)).ToList();
                folderFiles = folderFiles.Where(item => !listNotStart.Any(y => item.Name.StartsWith(y))).ToList();

                if (folderFiles.Any())
                    return true;
            }

            return false;
        }

        private async Task<bool> CheckFileName(string fileName, string regex)
        {
            Regex re = new Regex(regex);
            if (re.IsMatch(fileName))
                return true;

            return false;
        }

        public async Task ImportCashFile()
        {
            string label = "CashFile";
            var listNotStart = new List<string>();
            listNotStart.Add("OK_");
            listNotStart.Add("NOK_");

            LoggingHandler.LogInfo($"STARTING CASH FILE IMPORT");

            var batch = await _createBatchLogUseCase.ExecuteAsync(label).ConfigureAwait(false);
            var googleFileSettings = await GetGoogleFileSetting(label).ConfigureAwait(false);

            foreach (GoogleFileSettingDomain googleFileSettingItem in googleFileSettings)
            {
                IList<File> folderFiles = await _getFilesInGoogleDriveUseCase
                    .ExecuteAsync(googleFileSettingItem.GoogleIdentifier)
                    .ConfigureAwait(false);

                if (folderFiles.Any())
                {
                    folderFiles = folderFiles.Where(item => item.Name.EndsWith(googleFileSettingItem.FileType)).ToList();
                    folderFiles = folderFiles.Where(item => !listNotStart.Any(y => item.Name.StartsWith(y))).ToList();

                    LoggingHandler.LogInfo($"FOLDER ID: {googleFileSettingItem.GoogleIdentifier}");
                    LoggingHandler.LogInfo($"FILE COUNT: {folderFiles.Count}");

                    await HandleCashFile(batch.Id, folderFiles).ConfigureAwait(false);
                }
            }

            await _setBatchLogSuccessUseCase.ExecuteAsync(batch.Id).ConfigureAwait(false);
            LoggingHandler.LogInfo($"END CASH FILE IMPORT");
        }

        private async Task HandleCashFile(long batchId, IEnumerable<File> files)
        {
            foreach (File fileItem in files)
            {
                if (fileItem.Name.StartsWith("OK_") || fileItem.Name.StartsWith("NOK_"))
                    continue;

                LoggingHandler.LogInfo($"FILE NAME: {fileItem.Name}");
                try
                {
                    LoggingHandler.LogInfo($"CHECKING IF FILE NAME IS CORRECT");
                    var checkFileName = await CheckFileName(fileItem.Name, _cashFileRegex).ConfigureAwait(false);
                    if (!checkFileName)
                    {
                        LoggingHandler.LogWarning($"NON-STANDARD FILENAME (CashFileYYYYMMDD). CHECK FILE NAME ({fileItem.Name})");
                        await _createBatchLogErrorUseCase.ExecuteAsync(batchId, "WARNING", $"NON-STANDARD FILENAME (CashFileYYYYMMDD). CHECK FILE NAME ({fileItem.Name})").ConfigureAwait(false);
                        await _renameGoogleFileUseCase.ExecuteAsync(fileItem.Id, $"NOK_{fileItem.Name}").ConfigureAwait(false);
                        continue;
                    }

                    LoggingHandler.LogInfo($"CHECKING IF THE FILE HAS ALREADY LOADED");
                    UPCashFileNameDomain getResult = await _getUpCashFileNameUseCase.ExecuteAsync(fileItem.Name).ConfigureAwait(false);
                    if (getResult != null)
                    {
                        LoggingHandler.LogWarning($"FILE {fileItem.Name} ALREADY EXIST");
                        await _createBatchLogErrorUseCase.ExecuteAsync(batchId, "WARNING", $"FILE {fileItem.Name} ALREADY EXIST").ConfigureAwait(false);
                        await _renameGoogleFileUseCase.ExecuteAsync(fileItem.Id, $"NOK_{fileItem.Name}").ConfigureAwait(false);
                        continue;
                    }

                    LoggingHandler.LogInfo($"CREATING FILE ENTRY");
                    UPCashFileNameDomain createResult = await _createUpCashFileNameUseCase.ExecuteAsync(fileItem.Name).ConfigureAwait(false);
                    if (createResult == null)
                    {
                        LoggingHandler.LogWarning($"FILE ENTRY {fileItem.Name} NOT CREATED");
                        await _createBatchLogErrorUseCase.ExecuteAsync(batchId, "WARNING", $"FILE ENTRY {fileItem.Name} NOT CREATED").ConfigureAwait(false);
                        await _renameGoogleFileUseCase.ExecuteAsync(fileItem.Id, $"NOK_{fileItem.Name}").ConfigureAwait(false);
                        continue;
                    }

                    var fileLines = await _readGoogleFileLineDataUseCase
                        .ExecuteAsync(fileItem.Name, fileItem.Id, fileItem.MimeType)
                        .ConfigureAwait(false);
                    fileLines = fileLines.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();

                    LoggingHandler.LogInfo($"ROW COUNT: {fileLines.Count}");

                    var skip = 0;
                    var failure = false;
                    var batch = new List<string>();

                    do
                    {
                        batch = fileLines.Skip(skip).Take(_batchSize).ToList();
                        skip += _batchSize;

                        if (batch.Any())
                        {
                            var bulkResult = await _createBulkCashDumpsUseCase
                                .ExecuteAsync(createResult.Id, batch)
                                .ConfigureAwait(false);

                            if (bulkResult == null)
                            {
                                failure = true;

                                LoggingHandler.LogWarning($"FAILURE TO LOAD ALL ROWS: {createResult.Id}");
                                await _createBatchLogErrorUseCase.ExecuteAsync(batchId, "WARNING", $"FAILURE TO LOAD ALL ROWS: {createResult.Id}").ConfigureAwait(false);
                                await _renameGoogleFileUseCase.ExecuteAsync(fileItem.Id, $"NOK_{fileItem.Name}").ConfigureAwait(false);

                                continue;
                            }
                            LoggingHandler.LogInfo($"FILE LINES CREATED {bulkResult.Count} FOR FILE {createResult.Id}");
                        }
                    }
                    while (batch.Any() && !failure);

                    if (!failure)
                    {
                        LoggingHandler.LogInfo("FILE SUCCESS");
                        await _renameGoogleFileUseCase.ExecuteAsync(fileItem.Id, $"OK_{fileItem.Name}").ConfigureAwait(false);
                        await _setUpCashFileNameSuccessUseCase.ExecuteAsync(createResult.Id)
                            .ConfigureAwait(false);
                    }
                }
                catch (Exception exc)
                {
                    await _renameGoogleFileUseCase.ExecuteAsync(fileItem.Id, $"NOK_{fileItem.Name}").ConfigureAwait(false);
                    string namespaceLabel =
                        $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(HandleDatFileDownloads)}";

                    LoggingHandler.LogError($"{nameof(UPCashDumpFileName)} -- {nameof(UPCashDump)} -- {fileItem.Name}");
                    LoggingHandler.LogError($"{namespaceLabel} APPLICATION ERROR");
                    LoggingHandler.LogError(exc.ToString());

                    await _createBatchLogErrorUseCase.ExecuteAsync(batchId, "ERROR", $"{nameof(UPCashDumpFileName)} -- {nameof(UPCashDump)} -- {fileItem.Name}").ConfigureAwait(false);

                    throw;
                }
            }
        }

        public async Task LoadCashFileTransactions()
        {
            LoggingHandler.LogInfo($"CHECKING IF EXIST PENDING CASH FILES");
            var returnLoad = await _loadTransactionsUseCase.LoadCashFilesAsync().ConfigureAwait(false);
            LoggingHandler.LogInfo($"EXIST PENDING CASH FILES: {returnLoad}");
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

                            const int TAKE = 250;
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
                                    IList<UPCashDumpDomain> result = await _createBulkCashDumpsUseCase
                                        .ExecuteAsync(createResult.Id, batch)
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

                            const int TAKE = 250;
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
                                    IList<UPHousingCashDumpDomain> result = await _createBulkHousingCashDumpsUseCase
                                        .ExecuteAsync(createResult.Id, batch)
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
