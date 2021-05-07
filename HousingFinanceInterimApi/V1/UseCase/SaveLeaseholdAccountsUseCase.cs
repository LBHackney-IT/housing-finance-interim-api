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
    /// <seealso cref="ISaveLeaseholdAccountsUseCase" />
    public class SaveLeaseholdAccountsUseCase : ISaveLeaseholdAccountsUseCase
    {

        /// <summary>
        /// The automatic mapper
        /// </summary>
        private readonly IMapper _autoMapper;

        /// <summary>
        /// The gateway
        /// </summary>
        private readonly ILeaseholdAccountsGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveLeaseholdAccountsUseCase" /> class.
        /// </summary>
        /// <param name="autoMapper">The automatic mapper.</param>
        /// <param name="gateway">The gateway.</param>
        public SaveLeaseholdAccountsUseCase(IMapper autoMapper, ILeaseholdAccountsGateway gateway)
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
        public async Task<int> ExecuteAsync(IList<LeaseholdAccountDomain> items)
        {
            var databaseEntities = LeaseholdAccountsFactory.ToEntity(_autoMapper, items);

            return await _gateway.SaveLeaseholdAccountsItems(databaseEntities).ConfigureAwait(false);
        }

    }

}
