using Amazon.Lambda.Core;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
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
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Boundary.Response;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HousingFinanceInterimApi
{
    public class Handler
    {
        private readonly ICheckExistFileUseCase _checkExistFileUseCase;
        private readonly ICheckChargesBatchYearsUseCase _checkChargesBatchYearsUseCase;
        private readonly IGenerateRentPositionUseCase _generateRentPositionUseCase;
        private readonly IImportCashFileUseCase _importCashFileUseCase;
        private readonly IImportHousingFileUseCase _importHousingFileUseCase;
        private readonly ILoadActionDiaryUseCase _loadActionDiaryUseCase;
        private readonly ILoadAdjustmentUseCase _loadAdjustmentUseCase;
        private readonly ILoadCashFileTransactionsUseCase _loadCashFileTransactionsUseCase;
        private readonly ILoadChargesHistoryUseCase _loadChargesHistoryUseCase;
        private readonly ILoadChargesTransactionsUseCase _loadChargesTransactionsUseCase;
        private readonly ILoadChargesUseCase _loadChargesUseCase;
        private readonly ILoadDirectDebitTransactionsUseCase _loadDirectDebitTransactionsUseCase;
        private readonly ILoadDirectDebitUseCase _loadDirectDebitUseCase;
        private readonly ILoadHousingFileTransactionsUseCase _loadHousingFileTransactionsUseCase;
        private readonly ILoadTenancyAgreementUseCase _loadTenancyAgreementUseCase;
        private readonly IRefreshCurrentBalanceUseCase _refreshCurrentBalanceUseCase;
        private readonly IRefreshManageArrearsUseCase _refreshManageArrearsUseCase;
        private readonly IRefreshOperatingBalanceUseCase _refreshOperatingBalanceUseCase;

        private const string CashFileLabel = "CashFile";
        private const string HousingBenefitFileLabel = "HousingBenefitFile";

        public Handler()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
            DatabaseContext context = new DatabaseContext(optionsBuilder.Options);

            var options = Options.Create(new GoogleClientServiceOptions
            {
                ApplicationName = "Hackney Finance Interim Solution",
                Scopes = new List<string>
                {
                    DriveService.Scope.Drive, SheetsService.Scope.SpreadsheetsReadonly
                }
            });
            IGoogleClientService googleClientService = new GoogleClientServiceFactory(default, options, context)
                .CreateGoogleClientServiceFromJson(Environment.GetEnvironmentVariable("GOOGLE_API_KEY"));

            IActionDiaryGateway actionDiaryGateway = new ActionDiaryGateway(context);
            IAdjustmentGateway adjustmentGateway = new AdjustmentGateway(context);
            IBatchLogErrorGateway batchLogErrorGateway = new BatchLogErrorGateway(context);
            IBatchLogGateway batchLogGateway = new BatchLogGateway(context);
            IChargesBatchYearsGateway chargesBatchYearsGateway = new ChargesBatchYearsGateway(context);
            IChargesGateway chargesGateway = new ChargesGateway(context);
            ICurrentBalanceGateway currentBalanceGateway = new CurrentBalanceGateway(context);
            IDirectDebitGateway directDebitGateway = new DirectDebitGateway(context);
            IGoogleFileSettingGateway googleFileSettingGateway = new GoogleFileSettingGateway(context);
            IManageArrearsGateway manageArrearsGateway = new ManageArrearsGateway(context);
            IOperatingBalanceGateway operatingBalanceGateway = new OperatingBalanceGateway(context);
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
            _checkChargesBatchYearsUseCase = new CheckChargesBatchYearsUseCase(chargesBatchYearsGateway);
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
            _loadChargesHistoryUseCase = new LoadChargesHistoryUseCase(batchLogGateway, batchLogErrorGateway, chargesBatchYearsGateway, chargesGateway);
            _loadChargesTransactionsUseCase = new LoadChargesTransactionsUseCase(batchLogGateway, batchLogErrorGateway,
                chargesGateway, transactionGateway);
            _loadChargesUseCase = new LoadChargesUseCase(batchLogGateway, batchLogErrorGateway,
                chargesBatchYearsGateway, chargesGateway, googleFileSettingGateway, googleClientService);
            _loadDirectDebitTransactionsUseCase = new LoadDirectDebitTransactionsUseCase(batchLogGateway,
                batchLogErrorGateway, directDebitGateway, transactionGateway);
            _loadDirectDebitUseCase = new LoadDirectDebitUseCase(batchLogGateway, batchLogErrorGateway,
                directDebitGateway, googleFileSettingGateway, googleClientService);
            _loadHousingFileTransactionsUseCase = new LoadHousingFileTransactionsUseCase(batchLogGateway,
                batchLogErrorGateway, upHousingCashLoadGateway, transactionGateway);
            _loadTenancyAgreementUseCase = new LoadTenancyAgreementUseCase(batchLogGateway, batchLogErrorGateway,
                tenancyAgreementGateway, googleFileSettingGateway, googleClientService);
            _refreshCurrentBalanceUseCase = new RefreshCurrentBalanceUseCase(currentBalanceGateway);
            _refreshManageArrearsUseCase = new RefreshManageArrearsUseCase(manageArrearsGateway);
            _refreshOperatingBalanceUseCase = new RefreshOperatingBalanceUseCase(operatingBalanceGateway);
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

        public async Task<StepResponse> LoadDirectDebitTransactionsOnDemand(OnDemandRequest input)
        {
            return await _loadDirectDebitTransactionsUseCase.ExecuteOnDemandAsync(input.StartDate, input.EndDate).ConfigureAwait(false);
        }

        public async Task<StepResponse> CheckChargesBatchYears()
        {
            return await _checkChargesBatchYearsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadCharges()
        {
            return await _loadChargesUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadChargesHistory()
        {
            return await _loadChargesHistoryUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadChargesTransactions()
        {
            return await _loadChargesTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadChargesTransactionsOnDemand(OnDemandRequest input)
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

        public async Task<StepResponse> RefreshCurrentBalance()
        {
            return await _refreshCurrentBalanceUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> RefreshOperatingBalance()
        {
            return await _refreshOperatingBalanceUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> RefreshManageArrears()
        {
            return await _refreshManageArrearsUseCase.ExecuteAsync().ConfigureAwait(false);
        }
    }

}
