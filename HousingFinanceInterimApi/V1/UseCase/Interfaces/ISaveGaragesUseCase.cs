using HousingFinanceInterimApi.V1.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The save garage use case.
    /// </summary>
    public interface ISaveGaragesUseCase
    {

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The save result.</returns>
        public Task<int> ExecuteAsync(IList<GarageDomain> items);

    }

}
