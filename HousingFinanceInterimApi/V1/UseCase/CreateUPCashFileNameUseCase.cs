using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase
{

    public class CreateUPCashFileNameUseCase : ICreateUPCashFileNameUseCase
    {

        /// <summary>
        /// The gateway
        /// </summary>
        private readonly IUPCashFileNameGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUPCashFileNameUseCase"/> class.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        public CreateUPCashFileNameUseCase(IUPCashFileNameGateway gateway)
        {
            _gateway = gateway;
        }

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// The <see cref="UPCashFileNameDomain" /> instance
        /// </returns>
        public async Task<UPCashFileNameDomain> ExecuteAsync(string fileName)
            => UPCashFileNameFactory.ToDomain(await _gateway.CreateAsync(fileName).ConfigureAwait(false));

    }

}
