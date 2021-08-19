using System;
using System.Threading.Tasks;
using HousingFinanceInterimApi;

namespace AppCall
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var h = new Handler();
            await h.LoadDirectDebit().ConfigureAwait(false);
            //await h.LoadDirectDebitTransactions().ConfigureAwait(false);
            await h.LoadDirectDebitTransactions(new DateTime(2021, 05, 08)).ConfigureAwait(false);

        }
    }
}
