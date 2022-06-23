using System;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{
    public interface IReportGateway
    {
        Task<IList<ReportAccountBalance>> GetReportAccountBalanceAsync(DateTime reportDate, string rentGroup);
        Task<IList<string[]>> GetCashImportByDateAsync(DateTime startDate, DateTime endDate);
        Task<IList<string[]>> GetChargesByYearAndRentGroupAsync(int year, string rentGroup);
        Task<IList<string[]>> GetChargesByGroupTypeAsync(int year, string type);
        Task<IList<string[]>> GetChargesByYearAsync(int year);
        Task<IList<string[]>> GetCashSuspenseAccountByYearAsync(int year, string suspenseAccountType);
        Task<IList<string[]>> GetHousingBenefitAcademyByYearAsync(int year);
    }
}
