using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HousingFinanceInterimApi.V1.Controllers
{

    public class AuthController : BaseController
    {

        private readonly ICreateGoogleClientServiceUseCase _createGoogleClientServiceUseCase;

        public AuthController(ICreateGoogleClientServiceUseCase createGoogleClientServiceUseCase)
        {
            _createGoogleClientServiceUseCase = createGoogleClientServiceUseCase;
        }

        public async Task<IActionResult> AuthenticateGoogleAuthCode(string code)
        {
            // TODO
            IGoogleClientService googleClientService = _createGoogleClientServiceUseCase.Execute(code);

            return null;
        }

    }

}
