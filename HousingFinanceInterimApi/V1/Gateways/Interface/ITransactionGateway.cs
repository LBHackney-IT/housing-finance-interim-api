using System;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface ITransactionGateway
    {
        public Task<IList<Transaction>> ListAsync(DateTime? startDate, DateTime? endDate);

        public Task LoadCashFilesTransactions();

        public Task LoadHousingFilesTransactions();

        public Task LoadDirectDebitTransactions();

        public Task LoadChargesTransactions(int processingYear);

        public Task CreateCashFileSuspenseAccountTransaction(long id, string newRentAccount);

        public Task CreateHousingFileSuspenseAccountTransaction(long id, string newRentAccount);
        public Task CleanSSMiniTransactions();
    }
}
