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
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain.AutoMaps;
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
        private readonly ICheckExistFileUseCase _checkExistFileUseCase;
        private readonly IGenerateRentPositionUseCase _generateRentPositionUseCase;
        private readonly IImportCashFileUseCase _importCashFileUseCase;
        private readonly IImportHousingFileUseCase _importHousingFileUseCase;
        private readonly ILoadActionDiaryUseCase _loadActionDiaryUseCase;
        private readonly ILoadAdjustmentUseCase _loadAdjustmentUseCase;
        private readonly ILoadCashFileTransactionsUseCase _loadCashFileTransactionsUseCase;
        private readonly ILoadChargesTransactionsUseCase _loadChargesTransactionsUseCase;
        private readonly ILoadChargesUseCase _loadChargesUseCase;
        private readonly ILoadDirectDebitTransactionsUseCase _loadDirectDebitTransactionsUseCase;
        private readonly ILoadDirectDebitUseCase _loadDirectDebitUseCase;
        private readonly ILoadHousingFileTransactionsUseCase _loadHousingFileTransactionsUseCase;
        private readonly ILoadTenancyAgreementUseCase _loadTenancyAgreementUseCase;

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

        /// <summary>
        /// The google file settings list use case
        /// </summary>
        private readonly IListGoogleFileSettingsUseCase _googleFileSettingsList;

        private const string CashFileLabel = "CashFile";
        private const string HousingBenefitFileLabel = "HousingBenefitFile";

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

            // Housing File name use cases
            IUPHousingCashFileNameGateway housingFileNameGateway = new UPHousingCashFileNameGateway(context);
            _getUpHousingCashFileNameUseCase = new GetUPHousingCashFileNameUseCase(housingFileNameGateway);
            _createUpHousingCashFileNameUseCase = new CreateUPHousingCashFileNameUseCase(housingFileNameGateway);
            _setUpHousingCashFileNameSuccessUseCase = new SetUPHousingCashFileNameSuccessUseCase(housingFileNameGateway);

            // Google file setting use cases
            IGoogleFileSettingGateway settingGateway = new GoogleFileSettingGateway(context);
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

            // Google service use cases
            IGoogleClientService googleClientService =
                new GoogleClientServiceFactory(default, options, context)
                    .CreateGoogleClientServiceForApiKey(Environment.GetEnvironmentVariable("GOOGLE_API_KEY"));
            _getFilesInGoogleDriveUseCase = new GetFilesInGoogleDriveUseCase(googleClientService);
            _readGoogleFileLineDataUseCase = new ReadGoogleFileLineDataUseCase(googleClientService);
            _readGoogleSheetToEntitiesUseCase = new ReadGoogleSheetToEntities(googleClientService);

            IActionDiaryGateway actionDiaryGateway = new ActionDiaryGateway(context);
            IAdjustmentGateway adjustmentGateway = new AdjustmentGateway(context);
            IBatchLogErrorGateway batchLogErrorGateway = new BatchLogErrorGateway(context);
            IBatchLogGateway batchLogGateway = new BatchLogGateway(context);
            IChargesGateway chargesGateway = new ChargesGateway(context);
            IDirectDebitGateway directDebitGateway = new DirectDebitGateway(context);
            IGoogleFileSettingGateway googleFileSettingGateway = new GoogleFileSettingGateway(context);
            IRentPositionGateway rentPositionGateway = new RentPositionGateway(context);
            ITenancyAgreementGateway tenancyAgreementGateway = new TenancyAgreementGateway(context);
            ITransactionGateway transactionGateway = new TransactionGateway(context);
            IUPCashDumpFileNameGateway upCashDumpFileNameGateway = new UPCashDumpFileNameGateway(context);
            IUPCashDumpGateway upCashDumpGateway = new UPCashDumpGateway(context);
            IUPCashLoadGateway upCashLoadGateway = new UPCashLoadGateway(context);
            IUPHousingCashDumpFileNameGateway upHousingCashDumpFileNameGateway = new UPHousingCashDumpFileNameGateway(context);
            IUPHousingCashDumpGateway upHousingCashDumpGateway = new UPHousingCashDumpGateway(context);
            IUPHousingCashLoadGateway upHousingCashLoadGateway = new UPHousingCashLoadGateway(context);

            _checkExistFileUseCase = new CheckExistFileUseCase(googleFileSettingGateway, googleClientService);
            _generateRentPositionUseCase = new GenerateRentPositionUseCase(rentPositionGateway, batchLogGateway,
                batchLogErrorGateway, googleFileSettingGateway, googleClientService);
            _importCashFileUseCase = new ImportCashFileUseCase(batchLogGateway, batchLogErrorGateway,
                googleFileSettingGateway, googleClientService, upCashDumpFileNameGateway, upCashDumpGateway);
            _importHousingFileUseCase = new ImportHousingFileUseCase(batchLogGateway, batchLogErrorGateway,
                googleFileSettingGateway, googleClientService, upHousingCashDumpFileNameGateway, upHousingCashDumpGateway);
            _loadActionDiaryUseCase = new LoadActionDiaryUseCase(batchLogGateway, batchLogErrorGateway,
                actionDiaryGateway, googleFileSettingGateway, googleClientService);
            _loadAdjustmentUseCase = new LoadAdjustmentUseCase(batchLogGateway, batchLogErrorGateway, adjustmentGateway,
                googleFileSettingGateway, googleClientService);
            _loadCashFileTransactionsUseCase = new LoadCashFileTransactionsUseCase(batchLogGateway,
                batchLogErrorGateway, upCashLoadGateway, transactionGateway);
            _loadChargesTransactionsUseCase = new LoadChargesTransactionsUseCase(batchLogGateway, batchLogErrorGateway,
                chargesGateway, transactionGateway);
            _loadChargesUseCase = new LoadChargesUseCase(batchLogGateway, batchLogErrorGateway,
                chargesGateway, googleFileSettingGateway, googleClientService);
            _loadDirectDebitTransactionsUseCase = new LoadDirectDebitTransactionsUseCase(batchLogGateway,
                batchLogErrorGateway, directDebitGateway, transactionGateway);
            _loadDirectDebitUseCase = new LoadDirectDebitUseCase(batchLogGateway, batchLogErrorGateway,
                directDebitGateway, googleFileSettingGateway, googleClientService);
            _loadHousingFileTransactionsUseCase = new LoadHousingFileTransactionsUseCase(batchLogGateway,
                batchLogErrorGateway, upHousingCashLoadGateway, transactionGateway);
            _loadTenancyAgreementUseCase = new LoadTenancyAgreementUseCase(batchLogGateway, batchLogErrorGateway,
                tenancyAgreementGateway, googleFileSettingGateway, googleClientService);
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

        public async Task<StepResponse> LoadTenancyAgreement()
        {
            return await _loadTenancyAgreementUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> CheckCashFiles()
        {
            return await _checkExistFileUseCase.ExecuteAsync(CashFileLabel).ConfigureAwait(false);
        }

        public async Task<StepResponse> ImportCashFile()
        {
            return await _importCashFileUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadCashFileTransactions()
        {
            return await _loadCashFileTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> CheckHousingBenefitFiles()
        {
            return await _checkExistFileUseCase.ExecuteAsync(HousingBenefitFileLabel).ConfigureAwait(false);
        }

        public async Task<StepResponse> ImportHousingBenefitFile()
        {
            return await _importHousingFileUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadHousingBenefitFileTransactions()
        {
            return await _loadHousingFileTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadDirectDebit()
        {
            return await _loadDirectDebitUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadDirectDebitTransactions()
        {
            return await _loadDirectDebitTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadDirectDebitTransactionsOnDemand(OnDemandRequest input, ILambdaContext context)
        {
            return await _loadDirectDebitTransactionsUseCase.ExecuteOnDemandAsync(input.StartDate, input.EndDate).ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadCharges()
        {
            return await _loadChargesUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadChargesTransactions()
        {
            return await _loadChargesTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadChargesTransactionsOnDemand(OnDemandRequest input, ILambdaContext context)
        {
            return await _loadChargesTransactionsUseCase.ExecuteOnDemandAsync(input.StartDate, input.EndDate).ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadActionDiary()
        {
            return await _loadActionDiaryUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadAdjustmentsTransactions()
        {
            return await _loadAdjustmentUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> GenerateRentPosition()
        {
            return await _generateRentPositionUseCase.ExecuteAsync().ConfigureAwait(false);
        }
    }

}
