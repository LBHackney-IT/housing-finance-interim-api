using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase
{

    public class LoadTransactionsUseCase : ILoadTransactionsUseCase
    {
        private readonly ITransactionGateway _transactionGateway;
        private readonly IUPCashLoadGateway _upCashLoadGateway;
        private readonly IUPHousingCashLoadGateway _upHousingCashLoadGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadTransactionsUseCase"/> class.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        public LoadTransactionsUseCase(ITransactionGateway transactionGateway, IUPCashLoadGateway upCashLoadGateway, IUPHousingCashLoadGateway upHousingCashLoadGateway)
        {
            _transactionGateway = transactionGateway;
            _upCashLoadGateway = upCashLoadGateway;
            _upHousingCashLoadGateway = upHousingCashLoadGateway;
        }

        public async Task<bool> LoadCashFilesAsync()
        {
            var result = await _upCashLoadGateway.LoadCashFiles().ConfigureAwait(false);
            if (result)
                await _transactionGateway.LoadCashFilesTransactions().ConfigureAwait(false);

            return true;
        }

        public async Task<bool> LoadHousingFilesAsync()
        {
            var result = await _upHousingCashLoadGateway.LoadHousingFiles().ConfigureAwait(false);
            if (result)
                await _transactionGateway.LoadHousingFilesTransactions().ConfigureAwait(false);

            return true;
        }
    }

}
