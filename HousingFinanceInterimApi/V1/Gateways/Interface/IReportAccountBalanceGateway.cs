using System;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IReportAccountBalanceGateway
    {
        Task<IList<ReportAccountBalance>> ListAsync(DateTime reportDate, string rentGroup);
    }
}
