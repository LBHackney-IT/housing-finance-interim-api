using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;

namespace HousingFinanceInterimApi.V1.UseCase
{

    /// <summary>
    /// The create bulk housing cash dump use case.
    /// </summary>
    /// <seealso cref="ICreateBulkHousingCashDumpsUseCase" />
    public class CreateBulkHousingCashDumpsUseCase : ICreateBulkHousingCashDumpsUseCase
    {

        /// <summary>
        /// The gateway
        /// </summary>
        private readonly IUPHousingCashDumpGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateBulkHousingCashDumpsUseCase"/> class.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        public CreateBulkHousingCashDumpsUseCase(IUPHousingCashDumpGateway gateway)
        {
            _gateway = gateway;
        }

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>
        /// The created UP housing cash dump domain objects.
        /// </returns>
        public async Task<IList<UPHousingCashDumpDomain>> ExecuteAsync(long fileId, IList<string> lines)
            => UPHousingCashDumpFactory.ToDomain(await _gateway.CreateBulkAsync(fileId, lines).ConfigureAwait(false));

    }

}
