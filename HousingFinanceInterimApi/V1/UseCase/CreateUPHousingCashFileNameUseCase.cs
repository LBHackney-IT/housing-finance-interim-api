using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase
{

    public class CreateUPHousingCashFileNameUseCase : ICreateUPHousingCashFileNameUseCase
    {

        /// <summary>
        /// The gateway
        /// </summary>
        private readonly IUPHousingCashFileNameGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUPHousingCashFileNameUseCase"/> class.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        public CreateUPHousingCashFileNameUseCase(IUPHousingCashFileNameGateway gateway)
        {
            _gateway = gateway;
        }

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// The <see cref="UPHousingCashFileNameDomain" /> instance
        /// </returns>
        public async Task<UPHousingCashFileNameDomain> ExecuteAsync(string fileName)
            => UPHousingCashFileNameFactory.ToDomain(await _gateway.CreateAsync(fileName).ConfigureAwait(false));

    }

}
