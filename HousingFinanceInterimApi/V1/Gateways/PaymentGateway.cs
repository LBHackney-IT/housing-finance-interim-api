using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class PaymentGateway : IPaymentGateway
    {

        private readonly DatabaseContext _context;

        public PaymentGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lists the operating balances asynchronous.
        /// </summary>
        /// <param name="tenancyAgreementRef">The tenancy agreement reference.</param>
        /// <param name="rentAccount">The rent account number.</param>
        /// <param name="householdRef">The household reference.</param>
        /// <param name="count">Number of rows to return.</param>
        /// <param name="order">List order (ASC or DESC).</param>
        /// <returns>
        /// The list of operating balances.
        /// </returns>
        public async Task<IList<Payment>> ListAsync(string tenancyAgreementRef, string rentAccount, string householdRef, int count, string order)
        {
            var results = await _context.GetPaymentsAsync(tenancyAgreementRef, rentAccount, householdRef, count, order).ConfigureAwait(false);

            return results;
        }

    }

}
