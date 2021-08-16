using HousingFinanceInterimApi.V1.Gateways.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase
{

    public class RenameGoogleFileUseCase : IRenameGoogleFileUseCase
    {

        private readonly IGoogleClientService _googleClientService;

        public RenameGoogleFileUseCase(IGoogleClientService googleClientService)
        {
            _googleClientService = googleClientService;
        }

        public async Task<bool> ExecuteAsync(string fileId, string newName)
            => await _googleClientService.RenameFileInDrive(fileId, newName).ConfigureAwait(false);
    }
}
