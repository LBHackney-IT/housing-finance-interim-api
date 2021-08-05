using HousingFinanceInterimApi.V1.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The refresh manage arrears use case.
    /// </summary>
    public interface IRefreshManageArrearsUseCase
    {

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        public Task ExecuteAsync();

    }

}
