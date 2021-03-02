using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HousingFinanceInterimApi.V1.Controllers
{

    public class AuthController : BaseController
    {

        public AuthController()
        {
            
        }

        public async Task<IActionResult> AuthenticateGoogleAuthCode(string code)
        {
        }

    }

}
