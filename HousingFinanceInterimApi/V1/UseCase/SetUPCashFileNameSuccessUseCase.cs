using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;

namespace HousingFinanceInterimApi.V1.UseCase
{

    /// <summary>
    /// The set UP cash file name success use case implementation.
    /// </summary>
    /// <seealso cref="ISetUPCashFileNameSuccessUseCase" />
    public class SetUPCashFileNameSuccessUseCase : ISetUPCashFileNameSuccessUseCase
    {

        /// <summary>
        /// The UP cash file name gateway
        /// </summary>
        private readonly IUPCashFileNameGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetUPCashFileNameSuccessUseCase"/> class.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        public SetUPCashFileNameSuccessUseCase(IUPCashFileNameGateway gateway)
        {
            _gateway = gateway;
        }

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        public async Task ExecuteAsync(long fileId)
        {
            await _gateway.SetToSuccessAsync(fileId).ConfigureAwait(false);
        }

    }

}
