using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class ReportCashImportGateway : IReportCashImportGateway
    {

        private readonly DatabaseContext _context;

        public ReportCashImportGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IList<ReportCashImport>> ListCashImportByDateAsync(DateTime startDate, DateTime endDate)
        {
            var results = await _context.GetCashImportByDateAsync(startDate, endDate).ConfigureAwait(false);

            return results;
        }
    }

}
