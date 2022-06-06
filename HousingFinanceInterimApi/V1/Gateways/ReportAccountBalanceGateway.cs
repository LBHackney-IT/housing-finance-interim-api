using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class ReportAccountBalanceGateway : IReportAccountBalanceGateway
    {

        private readonly DatabaseContext _context;

        public ReportAccountBalanceGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IList<ReportAccountBalance>> ListAsync(DateTime reportDate, string rentGroup)
        {
            var results = await _context.GetReportAccountBalance(reportDate, rentGroup).ConfigureAwait(false);

            return results;
        }
    }

}
