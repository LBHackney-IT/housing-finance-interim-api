using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Domain.ArgumentWrappers;
using HousingFinanceInterimApi.V1.Factories;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class TransactionGateway : ITransactionGateway
    {

        private readonly IDatabaseContext _context;

        public TransactionGateway(IDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IList<Transaction>> ListAsync(DateTime? startDate, DateTime? endDate)
        {
            var results = await _context.GetTransactionsAsync(startDate, endDate).ConfigureAwait(false);

            return results;
        }

        public async Task<IList<PRNTransactionDomain>> GetPRNTransactions(GetPRNTransactionsDomain filterArguments)
        {
            var prnTransactions = await _context
                .GetPRNTransactionsByRentGroupAsync(
                    filterArguments.RentGroup,
                    filterArguments.FinancialYear,
                    filterArguments.StartWeekOrMonth,
                    filterArguments.EndWeekOrMonth
                )
                .ConfigureAwait(false);

            return prnTransactions.ToDomain();
        }

        public async Task LoadCashFilesTransactions()
        {
            try
            {
                await _context.LoadCashFileTransactions().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task LoadHousingFilesTransactions()
        {
            try
            {
                await _context.LoadHousingFileTransactions().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task LoadDirectDebitTransactions()
        {
            try
            {
                await _context.LoadDirectDebitTransactions().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task LoadChargesTransactions(int processingYear)
        {
            try
            {
                await _context.LoadChargesTransactions(processingYear).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task CreateCashFileSuspenseAccountTransaction(long id, string newRentAccount)
        {
            try
            {
                await _context.CreateCashFileSuspenseAccountTransaction(id, newRentAccount).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task CreateHousingFileSuspenseAccountTransaction(long id, string newRentAccount)
        {
            try
            {
                await _context.CreateHousingFileSuspenseAccountTransaction(id, newRentAccount).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }
    }
}
