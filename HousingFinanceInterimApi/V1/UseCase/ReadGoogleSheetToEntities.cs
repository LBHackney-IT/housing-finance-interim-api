using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase
{

    /// <summary>
    /// The read google sheet to entities use case implementation.
    /// </summary>
    /// <seealso cref="IReadGoogleSheetToEntities" />
    public class ReadGoogleSheetToEntities : IReadGoogleSheetToEntities
    {

        /// <summary>
        /// The google service
        /// </summary>
        private readonly IGoogleClientService _googleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadGoogleSheetToEntities"/> class.
        /// </summary>
        /// <param name="googleService">The google service.</param>
        public ReadGoogleSheetToEntities(IGoogleClientService googleService)
        {
            _googleService = googleService;
        }

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <typeparam name="_TEntity">The type of the t entity.</typeparam>
        /// <param name="spreadSheetId">The spread sheet identifier.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="sheetRange">The sheet range.</param>
        /// <returns>
        /// The serialized entities.
        /// </returns>
        public async Task<IList<_TEntity>> ExecuteAsync<_TEntity>(string spreadSheetId, string sheetName,
            string sheetRange) where _TEntity : class => await _googleService
            .ReadSheetToEntitiesAsync<_TEntity>(spreadSheetId, sheetName, sheetRange)
            .ConfigureAwait(false);

    }

}
