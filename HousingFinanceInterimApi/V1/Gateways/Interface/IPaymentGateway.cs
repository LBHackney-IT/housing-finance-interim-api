using System;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The operating balance gateway.
    /// </summary>
    public interface IPaymentGateway
    {

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
        public Task<IList<Payment>> ListAsync(string tenancyAgreementRef, string rentAccount, string householdRef, int count, string order);
    }

}
