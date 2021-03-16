using AutoMapper;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.UseCase
{

    /// <summary>
    /// The save rent breakdowns use case implementation.
    /// </summary>
    /// <seealso cref="ISaveRentBreakdownsUseCase" />
    public class SaveRentBreakdownsUseCase : ISaveRentBreakdownsUseCase
    {

        /// <summary>
        /// The automatic mapper
        /// </summary>
        private readonly IMapper _autoMapper;

        /// <summary>
        /// The gateway
        /// </summary>
        private readonly IRentBreakdownGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveRentBreakdownsUseCase" /> class.
        /// </summary>
        /// <param name="autoMapper">The automatic mapper.</param>
        /// <param name="gateway">The gateway.</param>
        public SaveRentBreakdownsUseCase(IMapper autoMapper, IRentBreakdownGateway gateway)
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
        public async Task<int> ExecuteAsync(IList<RentBreakdownDomain> items)
        {
            IList<RentBreakdown> entities = RentBreakdownFactory.ToEntity(_autoMapper, items);

            return await _gateway.SaveRentBreakdownItems(entities).ConfigureAwait(false);
        }

    }

}
