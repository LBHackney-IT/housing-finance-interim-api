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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Google.Apis.Logging;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;
using Mapster;
using Microsoft.Extensions.Http.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ILogger = Google.Apis.Logging.ILogger;
using LogLevel = Google.Apis.Logging.LogLevel;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HousingFinanceInterimApi
{
    public class Handler
    {
        private readonly IRefreshManageArrearsUseCase _refreshManageArrearsUseCase;
        private readonly IRefreshCurrentBalanceUseCase _refreshCurrentBalanceUseCase;
        private readonly ICheckExistFileUseCase _checkExistFileUseCase;
        private readonly IImportCashFileUseCase _importCashFileUseCase;
        private readonly ILoadCashFileTransactionsUseCase _loadCashFileTransactionsUseCase;
        private readonly IImportHousingFileUseCase _importHousingFileUseCase;
        private readonly ILoadHousingFileTransactionsUseCase _loadHousingFileTransactionsUseCase;
        private readonly ILoadDirectDebitUseCase _loadDirectDebitUseCase;
        private readonly ILoadDirectDebitTransactionsUseCase _loadDirectDebitTransactionsUseCase;

        private readonly string _cashFileLabel = "CashFile";
        private readonly string _housingBenefitFileLabel = "HousingBenefitFile";

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
        }

        public async Task<StepResponse> CheckCashFiles()
        {
            return await _checkExistFileUseCase.ExecuteAsync(_cashFileLabel).ConfigureAwait(false);
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
            return await _checkExistFileUseCase.ExecuteAsync(_housingBenefitFileLabel).ConfigureAwait(false);
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

        public async Task<StepResponse> RefreshManageArrears()
        {
            return await _refreshManageArrearsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadDirectDebit()
        {
            return await _loadDirectDebitUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadDirectDebitTransactions(JObject input, ILambdaContext context)
        {
            DateTime? processingDate = Convert.ToDateTime(input["processingDate"]);
            return await _loadDirectDebitTransactionsUseCase.ExecuteAsync(processingDate).ConfigureAwait(false);
        }
    }

}
