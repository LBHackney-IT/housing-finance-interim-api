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
    /// The save garage use case.
    /// </summary>
    /// <seealso cref="ISaveOtherHRAUseCase" />
    public class SaveOtherHRAUseCase : ISaveOtherHRAUseCase
    {

        /// <summary>
        /// The automatic mapper
        /// </summary>
        private readonly IMapper _autoMapper;

        /// <summary>
        /// The gateway
        /// </summary>
        private readonly IOtherHRAGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveOtherHRAUseCase" /> class.
        /// </summary>
        /// <param name="autoMapper">The automatic mapper.</param>
        /// <param name="gateway">The gateway.</param>
        public SaveOtherHRAUseCase(IMapper autoMapper, IOtherHRAGateway gateway)
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
        public async Task<int> ExecuteAsync(IList<OtherHRADomain> items)
        {
            var databaseEntities = OtherHRAFactory.ToEntity(_autoMapper, items);

            return await _gateway.SaveOtherHRAItems(databaseEntities).ConfigureAwait(false);
        }

    }

}
