using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class TenancyGateway : ITenancyGateway
    {

        private readonly DatabaseContext _context;

        public TenancyGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lists the operating balances asynchronous.
        /// </summary>
        /// <param name="tenancyAgreementRef">The tenancy agreement reference.</param>
        /// <param name="rentAccount">The rent account number.</param>
        /// <param name="householdRef">The household reference.</param>
        /// <returns>
        /// The list of operating balances.
        /// </returns>
        public async Task<Tenancy> GetAsync(string tenancyAgreementRef, string rentAccount, string householdRef)
        {
            var results = await _context.GetTenanciesAsync(tenancyAgreementRef, rentAccount, householdRef).ConfigureAwait(false);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Lists the tenancy transactions.
        /// </summary>
        /// <param name="tenancyAgreementRef">The tenancy agreement reference.</param>
        /// <param name="rentAccount">The rent account number.</param>
        /// <param name="householdRef">The household reference.</param>
        /// <param name="count">Number of rows to return.</param>
        /// <param name="order">List order (ASC or DESC).</param>
        /// <returns>
        /// The list of transactions.
        /// </returns>
        public async Task<IList<TenancyTransaction>> GetTransactionsAsync(string tenancyAgreementRef, string rentAccount, string householdRef, int count, string order)
        {
            var results = await _context.GetTenancyTransactionsAsync(tenancyAgreementRef, rentAccount, householdRef, count, order).ConfigureAwait(false);
            return results;
        }
    }
}
