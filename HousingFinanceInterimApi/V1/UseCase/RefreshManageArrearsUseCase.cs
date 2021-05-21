using AutoMapper;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase
{

    /// <summary>
    /// The refresh manage arrears use case.
    /// </summary>
    /// <seealso cref="IRefreshManageArrearsUseCase" />
    public class RefreshManageArrearsUseCase : IRefreshManageArrearsUseCase
    {
        /// <summary>
        /// The gateway
        /// </summary>
        private readonly IRefreshManageArrearsGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshManageArrearsUseCase" /> class.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        public RefreshManageArrearsUseCase(IRefreshManageArrearsGateway gateway)
        {
            _gateway = gateway;
        }

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        public async Task ExecuteAsync()
        {
            await _gateway.RefreshManageArrearsItems().ConfigureAwait(false);
        }

    }

}
