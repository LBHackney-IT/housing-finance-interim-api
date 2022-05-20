using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class ReportSuspenseAccountGateway : IReportSuspenseAccountGateway
    {

        private readonly DatabaseContext _context;

        public ReportSuspenseAccountGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IList<ReportCashSuspenseAccount>> ListCashSuspenseByYearAndTypeAsync(int year, string suspenseAccountType)
        {
            var results = await _context.GetCashSuspenseAccountByYearAsync(year, suspenseAccountType).ConfigureAwait(false);

            return results;
        }
    }

}
