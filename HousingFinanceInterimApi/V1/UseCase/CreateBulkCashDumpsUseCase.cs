using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;

namespace HousingFinanceInterimApi.V1.UseCase
{

    /// <summary>
    /// The create bulk cash dump use case.
    /// </summary>
    /// <seealso cref="ICreateBulkCashDumpsUseCase" />
    public class CreateBulkCashDumpsUseCase : ICreateBulkCashDumpsUseCase
    {

        /// <summary>
        /// The gateway
        /// </summary>
        private readonly IUPCashDumpGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateBulkCashDumpsUseCase"/> class.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        public CreateBulkCashDumpsUseCase(IUPCashDumpGateway gateway)
        {
            _gateway = gateway;
        }

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>
        /// The created UP cash dump domain objects.
        /// </returns>
        public async Task<IList<UPCashDumpDomain>> ExecuteAsync(long fileId, IList<string> lines)
            => UPCashDumpFactory.ToDomain(await _gateway.CreateBulkAsync(fileId, lines).ConfigureAwait(false));

    }

}
