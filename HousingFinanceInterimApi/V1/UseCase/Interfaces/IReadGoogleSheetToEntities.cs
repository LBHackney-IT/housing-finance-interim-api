using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The read google sheet to entities use case interface.
    /// </summary>
    public interface IReadGoogleSheetToEntities
    {

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="spreadSheetId">The spread sheet identifier.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="sheetRange">The sheet range.</param>
        /// <returns>The serialized entities.</returns>
        public Task<IList<TEntity>> ExecuteAsync<TEntity>(string spreadSheetId, string sheetName, string sheetRange)
            where TEntity : class;

    }

}
