using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IRenameGoogleFileUseCase
    {
        public Task<bool> ExecuteAsync(string fileId, string newName);
    }
}
