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

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadTransactionsUseCase"/> class.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        public LoadTransactionsUseCase(ITransactionGateway transactionGateway, IUPCashLoadGateway upCashLoadGateway)
        {
            _transactionGateway = transactionGateway;
            _upCashLoadGateway = upCashLoadGateway;
        }

        public async Task<bool> ExecuteAsync()
        {
            var result = await _upCashLoadGateway.LoadCashFiles().ConfigureAwait(false);
            if (result)
                await _transactionGateway.LoadCashFilesTransactions().ConfigureAwait(false);

            return true;
        }
    }

}
