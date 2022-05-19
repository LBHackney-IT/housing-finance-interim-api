using System;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IReportChargesGateway
    {

        Task<IList<dynamic>> ListByYearAndRentGroupAsync(int year, string rentGroup);
        Task<IList<dynamic>> ListByGroupTypeAsync(int year, string type);
        Task<IList<dynamic>> ListByYearAsync(int year);

    }

}
