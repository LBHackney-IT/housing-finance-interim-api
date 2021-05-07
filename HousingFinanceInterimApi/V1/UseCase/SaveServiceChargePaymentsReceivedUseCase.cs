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
    /// The save current rent positions use case.
    /// </summary>
    /// <seealso cref="ISaveServiceChargePaymentsReceivedUseCase" />
    public class SaveServiceChargePaymentsReceivedUseCase : ISaveServiceChargePaymentsReceivedUseCase
    {

        /// <summary>
        /// The automatic mapper
        /// </summary>
        private readonly IMapper _autoMapper;

        /// <summary>
        /// The gateway
        /// </summary>
        private readonly IServiceChargePaymentsReceivedGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveServiceChargePaymentsReceivedUseCase" /> class.
        /// </summary>
        /// <param name="autoMapper">The automatic mapper.</param>
        /// <param name="gateway">The gateway.</param>
        public SaveServiceChargePaymentsReceivedUseCase(IMapper autoMapper, IServiceChargePaymentsReceivedGateway gateway)
        {
            _autoMapper = autoMapper;
            _gateway = gateway;
        }

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>
        /// The save result.
        /// </returns>
        public async Task<int> ExecuteAsync(IList<ServiceChargePaymentsReceivedDomain> items)
        {
            var databaseEntities = ServiceChargePaymentsReceivedFactory.ToEntity(_autoMapper, items);

            return await _gateway.SaveServiceChargePaymentsReceivedItems(databaseEntities).ConfigureAwait(false);
        }

    }

}
