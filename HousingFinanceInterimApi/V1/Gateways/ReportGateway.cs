using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class ReportGateway : IReportGateway
    {

        private readonly DatabaseContext _context;

        public ReportGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IList<ReportAccountBalance>> GetReportAccountBalanceAsync(DateTime reportDate, string rentGroup)
        {
            var results = await _context.GetReportAccountBalance(reportDate, rentGroup).ConfigureAwait(false);

            return results;
        }

        public async Task<IList<string[]>> GetCashImportByDateAsync(DateTime startDate, DateTime endDate)
        {
            var results = await _context.GetCashImportByDateAsync(startDate, endDate).ConfigureAwait(false);

            return results;
        }

        public async Task<IList<string[]>> GetChargesByYearAndRentGroupAsync(int year, string rentGroup)
        {
            var results = await _context.GetChargesByYearAndRentGroupAsync(year, rentGroup).ConfigureAwait(false);

            return results;
        }

        public async Task<IList<string[]>> GetChargesByGroupTypeAsync(int year, string type)
        {
            var results = await _context.GetChargesByGroupType(year, type).ConfigureAwait(false);

            return results;
        }

        public async Task<IList<string[]>> GetChargesByYearAsync(int year)
        {
            var results = await _context.GetChargesByYear(year).ConfigureAwait(false);

            return results;
        }

        public async Task<IList<string[]>> GetCashSuspenseAccountByYearAsync(int year, string suspenseAccountType)
        {
            var results = await _context.GetCashSuspenseAccountByYearAsync(year, suspenseAccountType).ConfigureAwait(false);

            return results;
        }

        public async Task<IList<string[]>> GetHousingBenefitAcademyByYearAsync(int year)
        {
            var results = await _context.GetHousingBenefitAcademyByYear(year).ConfigureAwait(false);

            return results;
        }
    }

}
