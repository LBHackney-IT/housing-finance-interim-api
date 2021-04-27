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
    /// <seealso cref="ISaveGaragesUseCase" />
    public class SaveGaragesUseCase : ISaveGaragesUseCase
    {

        /// <summary>
        /// The automatic mapper
        /// </summary>
        private readonly IMapper _autoMapper;

        /// <summary>
        /// The gateway
        /// </summary>
        private readonly IGarageGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveGaragesUseCase" /> class.
        /// </summary>
        /// <param name="autoMapper">The automatic mapper.</param>
        /// <param name="gateway">The gateway.</param>
        public SaveGaragesUseCase(IMapper autoMapper, IGarageGateway gateway)
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
        public async Task<int> ExecuteAsync(IList<GarageDomain> items)
        {
            var databaseEntities = GarageFactory.ToEntity(_autoMapper, items);

            return await _gateway.SaveGarageItems(databaseEntities).ConfigureAwait(false);
        }

    }

}
