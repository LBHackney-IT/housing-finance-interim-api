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
using Amazon.Lambda.APIGatewayEvents;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Boundary.Response;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HousingFinanceInterimApi
{
    public class Handler
    {
        private readonly IRefreshManageArrearsUseCase _refreshManageArrearsUseCase;
        private readonly IRefreshCurrentBalanceUseCase _refreshCurrentBalanceUseCase;
        private readonly IRefreshOperatingBalanceUseCase _refreshOperatingBalanceUseCase;
        private readonly ICheckExistFileUseCase _checkExistFileUseCase;
        private readonly IImportCashFileUseCase _importCashFileUseCase;
        private readonly ILoadCashFileTransactionsUseCase _loadCashFileTransactionsUseCase;
        private readonly IImportHousingFileUseCase _importHousingFileUseCase;
        private readonly ILoadHousingFileTransactionsUseCase _loadHousingFileTransactionsUseCase;
        private readonly ILoadDirectDebitUseCase _loadDirectDebitUseCase;
        private readonly ILoadDirectDebitTransactionsUseCase _loadDirectDebitTransactionsUseCase;
        private readonly ILoadChargesUseCase _loadChargesUseCase;
        private readonly ILoadTenancyAgreementUseCase _loadTenancyAgreementUseCase;
        private readonly ILoadChargesTransactionsUseCase _loadChargesTransactionsUseCase;
        private readonly ILoadActionDiaryUseCase _loadActionDiaryUseCase;

        private const string CashFileLabel = "CashFile";
        private const string HousingBenefitFileLabel = "HousingBenefitFile";

        public Handler()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

            var context = new DatabaseContext(optionsBuilder.Options);
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

            IBatchLogErrorGateway batchLogErrorGateway = new BatchLogErrorGateway(context);
            IBatchLogGateway batchLogGateway = new BatchLogGateway(context);
            IManageArrearsGateway refreshManageArrearsGateway = new ManageArrearsGateway(context);
            ITransactionGateway transactionGateway = new TransactionGateway(context);
            IUPCashLoadGateway upCashLoadGateway = new UPCashLoadGateway(context);
            IUPHousingCashLoadGateway upHousingCashLoadGateway = new UPHousingCashLoadGateway(context);
            IGoogleFileSettingGateway googleFileSettingGateway = new GoogleFileSettingGateway(context);
            IUPCashDumpFileNameGateway upCashDumpFileNameGateway = new UPCashDumpFileNameGateway(context);
            IUPCashDumpGateway upCashDumpGateway = new UPCashDumpGateway(context);
            IUPHousingCashDumpFileNameGateway upHousingCashDumpFileNameGateway = new UPHousingCashDumpFileNameGateway(context);
            IUPHousingCashDumpGateway upHousingCashDumpGateway = new UPHousingCashDumpGateway(context);
            ICurrentBalanceGateway currentBalanceGateway = new CurrentBalanceGateway(context);
            IDirectDebitGateway directDebitGateway = new DirectDebitGateway(context);
            IChargesGateway chargesGateway = new ChargesGateway(context);
            ITenancyAgreementGateway tenancyAgreementGateway = new TenancyAgreementGateway(context);
            IOperatingBalanceGateway operatingBalanceGateway = new OperatingBalanceGateway(context);
            IActionDiaryGateway actionDiaryGateway = new ActionDiaryGateway(context);

            _checkExistFileUseCase = new CheckExistFileUseCase(googleFileSettingGateway, googleClientService);
            _importCashFileUseCase = new ImportCashFileUseCase(batchLogGateway, batchLogErrorGateway,
                googleFileSettingGateway, googleClientService, upCashDumpFileNameGateway, upCashDumpGateway);
            _loadCashFileTransactionsUseCase = new LoadCashFileTransactionsUseCase(batchLogGateway,
                batchLogErrorGateway, upCashLoadGateway, transactionGateway);
            _importHousingFileUseCase = new ImportHousingFileUseCase(batchLogGateway, batchLogErrorGateway,
                googleFileSettingGateway, googleClientService, upHousingCashDumpFileNameGateway, upHousingCashDumpGateway);
            _loadHousingFileTransactionsUseCase = new LoadHousingFileTransactionsUseCase(batchLogGateway,
                batchLogErrorGateway, upHousingCashLoadGateway, transactionGateway);
            _refreshCurrentBalanceUseCase = new RefreshCurrentBalanceUseCase(currentBalanceGateway);
            _refreshManageArrearsUseCase = new RefreshManageArrearsUseCase(refreshManageArrearsGateway);
            _loadDirectDebitUseCase = new LoadDirectDebitUseCase(batchLogGateway, batchLogErrorGateway,
                directDebitGateway, googleFileSettingGateway, googleClientService);
            _loadDirectDebitTransactionsUseCase = new LoadDirectDebitTransactionsUseCase(batchLogGateway,
                batchLogErrorGateway, directDebitGateway, transactionGateway);
            _loadChargesUseCase = new LoadChargesUseCase(batchLogGateway, batchLogErrorGateway,
                chargesGateway, googleFileSettingGateway, googleClientService);
            _loadTenancyAgreementUseCase = new LoadTenancyAgreementUseCase(batchLogGateway, batchLogErrorGateway,
                tenancyAgreementGateway, googleFileSettingGateway, googleClientService);
            _loadChargesTransactionsUseCase = new LoadChargesTransactionsUseCase(batchLogGateway, batchLogErrorGateway,
                chargesGateway, transactionGateway);
            _refreshOperatingBalanceUseCase = new RefreshOperatingBalanceUseCase(operatingBalanceGateway);
            _loadActionDiaryUseCase = new LoadActionDiaryUseCase(batchLogGateway, batchLogErrorGateway,
                actionDiaryGateway, googleFileSettingGateway, googleClientService);
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

        public async Task<StepResponse> LoadDirectDebit()
        {
            return await _loadDirectDebitUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadDirectDebitTransactions()
        {
            return await _loadDirectDebitTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadCharges()
        {
            return await _loadChargesUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadChargesTransactions()
        {
            return await _loadChargesTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<OnDemandRequest> LoadChargesOnDemand(OnDemandRequest input, ILambdaContext context)
        {
            return input;
        }

        public async Task<StepResponse> LoadTenancyAgreement()
        {
            return await _loadTenancyAgreementUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadActionDiary()
        {
            return await _loadActionDiaryUseCase.ExecuteAsync().ConfigureAwait(false);
        }
    }
}
