using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class ActionDiaryGateway : IActionDiaryGateway
    {
        private readonly DatabaseContext _context;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));

        public ActionDiaryGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateBulkAsync(IList<ActionDiaryAuxDomain> actionsDiaryDomain)
        {
            try
            {
                var actionsDiaryAux = actionsDiaryDomain.Select(ad => new ActionDiaryAux
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

                await _context.BulkInsertAsync(actionsDiaryAux, new BulkConfig { BatchSize = _batchSize }).ConfigureAwait(false);
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
    }
}
