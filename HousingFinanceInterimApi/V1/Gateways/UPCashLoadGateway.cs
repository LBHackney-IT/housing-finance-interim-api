using System;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Handlers;
using System.Linq;
using EFCore.BulkExtensions;
using HousingFinanceInterimApi.V1.Exceptions;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The UP Cash load gateway implementation.
    /// </summary>
    /// <seealso cref="IUPCashLoadGateway" />
    public class UPCashLoadGateway : IUPCashLoadGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCashLoadGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UPCashLoadGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task LoadCashFiles()
        {
            try
            {
                // Clear cash dumps for filenames that failed to process
                var failedFileNameIds = _context.UpCashDumpFileNames
                    .Where(fileName => !fileName.IsSuccess)
                    .Select(fileName => fileName.Id)
                    .ToList();
                _context.UpCashDumps
                    .Where(cashDump => failedFileNameIds.Contains(cashDump.UPCashDumpFileNameId))
                    .BatchDelete();

                var unreadCashDumps = _context.UpCashDumps.Where(cashDump => !cashDump.IsRead).ToList();

                var newCashLoads = unreadCashDumps.Select(CashDumpFromCashLoad).ToList();

                newCashLoads.ForEach(x => _context.UpCashLoads.Add(x));

                await _context.SaveChangesAsync().ConfigureAwait(false);

                // Mark processed cash dumps as read
                var newCashLoadIds = newCashLoads.Select(cashLoad => cashLoad.UPCashDumpId).ToList();
                _context.UpCashDumps
                    .Where(cashDump => newCashLoadIds.Contains(cashDump.Id))
                    .ToList()
                    .ForEach(x => x.IsRead = true);

                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (ArgumentOutOfRangeException e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw new InvalidCashFileTextException(e.Message);
            }
            catch (FormatException e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw new InvalidCashFileTextException(e.Message);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        private static UPCashLoad CashDumpFromCashLoad(UPCashDump cashDump)
        {
            return new UPCashLoad
            {
                RentAccount = cashDump.FullText[..10].Trim(),
                PaymentSource = cashDump.FullText[10..30].Trim(),
                MethodOfPayment = cashDump.FullText[30..33].Trim(),
                AmountPaid = Math.Round(decimal.Parse(cashDump.FullText[33..43]), 2),
                DatePaid = DateTime.ParseExact(cashDump.FullText[43..53], "dd/MM/yyyy", null),
                CivicaCode = cashDump.FullText[53..55].Trim(),
                IsRead = false,
                UPCashDumpId = cashDump.Id
            };
        }
    }

}
