using System;
using System.Threading.Tasks;
using Google.Apis.Util.Store;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The Google entity data store implementation.
    /// </summary>
    /// <seealso cref="IDataStore" />
    public class GoogleEntityDataStore : IDataStore
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleEntityDataStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public GoogleEntityDataStore(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Stores the given entity asynchronous.
        /// </summary>
        /// <typeparam name="_T">The type of the entity.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The entity value.</param>
        /// <returns>A task.</returns>
        public Task StoreAsync<_T>(string key, _T value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the entity at the given key asynchronous.
        /// </summary>
        /// <typeparam name="_T">The type of the entity.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>A task.</returns>
        public Task DeleteAsync<_T>(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="_T">The type of the entity.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>A task of type _T.</returns>
        public Task<_T> GetAsync<_T>(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously clears all values in the data store.
        /// </summary>
        /// <returns>A task.</returns>
        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

    }

}
