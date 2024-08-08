using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain.Reports;
using Microsoft.EntityFrameworkCore;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class ReportGateway : IReportGateway
    {

        private readonly IDatabaseContext _context;

        public ReportGateway(IDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IList<string[]>> GetReportAccountBalanceAsync(DateTime reportDate, string rentGroup)
        {
            var results = await _context.GetReportAccountBalance(reportDate, rentGroup).ConfigureAwait(false);

            return results;
        }

        public async Task<IList<string[]>> GetCashImportByDateAsync(DateTime startDate, DateTime endDate)
        {
            var reportOut = new List<string[]>();
            var headerRow = typeof(CashImportReport).GetProperties().Select(x => x.Name).ToArray();

            var cashLoadAmountsByDate = await _context.UpCashLoads
                .Join(
                    _context.UpCashDumps,
                    load => load.UPCashDumpId,
                    dump => dump.Id,
                    (load, dump) => new { load, dump }
                )
                .Join(
                    _context.UpCashDumpFileNames,
                    loadDump => loadDump.dump.UPCashDumpFileNameId,
                    fileName => fileName.Id,
                    (loadDump, fileName) => new { loadDump, fileName }
                )
                .GroupBy(x => new { x.fileName.Id, x.fileName.FileName })
                .Select(x => new
                {
                    x.Key.Id,
                    FileDate = DateTime.ParseExact(x.Key.FileName.Substring(8, 8), "yyyyMMdd", null),
                    Amount = x.Sum(y => y.loadDump.load.AmountPaid)
                })
                .ToListAsync().ConfigureAwait(false);

            var cashTransactions = await _context.SSMiniTransactions
                .Where(x => new List<string>
                {
                    "Cash File",
                    "Cash File (New Tenancy)",
                    "Cash File Suspense Account Reverse",
                    "Cash File Suspense Account Transfer"
                }.Contains(x.OriginDesc))
                .Where(x => x.PostDate >= startDate && x.PostDate <= endDate)
                .GroupBy(x => new
                {
                    RentGroup = x.TagRef == "SSSSSS" || x.TagRef == "ZZZZZZ" // Suspense accounts
                        ? x.TagRef.Substring(0, 3) // i.e. "SSS" or "ZZZ" for suspaccs
                        : x.RentGroup,
                    x.PostDate
                })
                .Select(x => new { x.Key.RentGroup, x.Key.PostDate, RealValue = x.Sum(y => y.RealValue) })
                .ToListAsync().ConfigureAwait(false);

                var results = cashTransactions
                .GroupBy(x => x.PostDate)
                .Select(x => new CashImportReport
                {
                    Date = x.Key,
                    IFSTotal = x.Sum(y => y.RealValue),
                    FileTotal = -1 * cashLoadAmountsByDate
                        .Where(y => y.FileDate == x.Key)
                        .Select(y => y.Amount)
                        .FirstOrDefault(),
                    GPS = x.Where(y => y.RentGroup == "GPS").Select(y => y.RealValue).FirstOrDefault(),
                    HGF = x.Where(y => y.RentGroup == "HGF").Select(y => y.RealValue).FirstOrDefault(),
                    HRA = x.Where(y => y.RentGroup == "HRA").Select(y => y.RealValue).FirstOrDefault(),
                    LMW = x.Where(y => y.RentGroup == "LMW").Select(y => y.RealValue).FirstOrDefault(),
                    LSC = x.Where(y => y.RentGroup == "LSC").Select(y => y.RealValue).FirstOrDefault(),
                    TAG = x.Where(y => y.RentGroup == "TAG").Select(y => y.RealValue).FirstOrDefault(),
                    TAH = x.Where(y => y.RentGroup == "TAH").Select(y => y.RealValue).FirstOrDefault(),
                    TRA = x.Where(y => y.RentGroup == "TRA").Select(y => y.RealValue).FirstOrDefault(),
                    ZZZZZZ = x.Where(y => y.RentGroup == "ZZZ").Select(y => y.RealValue).FirstOrDefault(),
                    SSSSSS = x.Where(y => y.RentGroup == "SSS").Select(y => y.RealValue).FirstOrDefault()
                })
                .OrderBy(x => x.Date)
                .Select(x => x.ToRow())
                .ToList();

            reportOut.Add(headerRow);
            reportOut.AddRange(results);

            return reportOut;
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

        public async Task<IList<string[]>> GetItemisedTransactionsByYearAndTransactionTypeAsync(int year, string transactionType)
        {
            var results = await _context
                .GetItemisedTransactionsByYearAndTransactionTypeAsync(year, transactionType)
                .ConfigureAwait(false);

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
