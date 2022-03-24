using System;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The tenancy gateway.
    /// </summary>
    public interface ITenancyGateway
    {

        /// <summary>
        /// Get tenancy details asynchronous.
        /// </summary>
        /// <param name="tenancyAgreementRef">The tenancy agreement reference.</param>
        /// <param name="rentAccount">The rent account number.</param>
        /// <param name="householdRef">The household reference.</param>
        /// <returns>
        /// The tenancy detaild
        /// </returns>
        public Task<Tenancy> GetAsync(string tenancyAgreementRef, string rentAccount, string householdRef);


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
        public Task<IList<TenancyTransaction>> GetTransactionsAsync(string tenancyAgreementRef, string rentAccount,
            string householdRef, int count, string order);

        /// <summary>
        /// Lists the tenancy transactions.
        /// </summary>
        /// <param name="tenancyAgreementRef">The tenancy agreement reference.</param>
        /// <param name="rentAccount">The rent account number.</param>
        /// <param name="householdRef">The household reference.</param>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <returns>
        /// The list of transactions.
        /// </returns>
        public Task<IList<TenancyTransaction>> GetTransactionsByDateAsync(string tenancyAgreementRef,
            string rentAccount, string householdRef, DateTime startDate, DateTime endDate);

        public Task<IList<DailyTransaction>> GetAllTransactionsAsync(string tenancyAgreementRef,
            string rentAccount, string householdRef);
    }

}
