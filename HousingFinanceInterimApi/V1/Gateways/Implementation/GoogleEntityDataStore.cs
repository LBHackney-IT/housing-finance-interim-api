using Google.Apis.Util.Store;
using System;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Implementation
{

    public class GoogleEntityDataStore : IDataStore
    {

        public Task StoreAsync<_T>(string key, _T value)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<_T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<_T> GetAsync<_T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

    }

}
