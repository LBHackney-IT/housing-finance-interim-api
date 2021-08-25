using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class ActionDiaryGateway : IActionDiaryGateway
    {
        private readonly DatabaseContext _context;

        public ActionDiaryGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<ActionDiaryAuxDomain>> CreateBulkAsync(IList<ActionDiaryAuxDomain> actionsDiaryDomain)
        {
            try
            {
                var actionsDiary = actionsDiaryDomain.Select(ad => new ActionDiaryAux
                {
                    TenancyAgreementRef = ad.TenancyAgreementRef,
                    RentAccount = ad.RentAccount,
                    ActionCode = ad.ActionCode,
                    Action = ad.Action,
                    ActionDate = ad.ActionDate,
                    Username = ad.Username,
                    ActionComment = ad.ActionComment,
                    Balance = ad.Balance
                }).ToList();

                _context.ActionsDiaryAux.AddRange(actionsDiary);
                bool success = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;

                return success
                    ? actionsDiary.ToDomain()
                    : null;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task ClearActionDiaryAuxiliary()
        {
            try
            {
                await _context.TruncateActionDiaryAuxiliary().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task LoadActionDiary()
        {
            try
            {
                await _context.LoadActionDiary().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        //public async Task LoadActionDiaryHistory(DateTime? processingDate)
        //{
        //    try
        //    {
        //        await _context.LoadActionDiaryHistory(processingDate).ConfigureAwait(false);
        //    }
        //    catch (Exception e)
        //    {
        //        LoggingHandler.LogError(e.Message);
        //        LoggingHandler.LogError(e.StackTrace);
        //        throw;
        //    }
        //}
    }
}
