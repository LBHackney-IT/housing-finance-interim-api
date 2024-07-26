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

                decimal parseAmountPaid(string amountPaidSubstring)
                {
                    // Example: +0000000001.00
                    bool isPositive = amountPaidSubstring[0] == '+';
                    var amountString = amountPaidSubstring[1..];
                    var amount = Math.Round(decimal.Parse(amountString), 2);
                    if (!isPositive)
                        amount *= -1;
                    return amount;
                }

                var newCashLoads = unreadCashDumps
                    .Select(cashDump => new UPCashLoad
                    {
                        RentAccount = cashDump.FullText.Substring(0, 10).Trim(),
                        PaymentSource = cashDump.FullText.Substring(10, 20).Trim(),
                        MethodOfPayment = cashDump.FullText.Substring(30, 3).Trim(),
                        AmountPaid = parseAmountPaid(cashDump.FullText.Substring(33, 10)),
                        DatePaid = DateTime.ParseExact(cashDump.FullText.Substring(43, 10), "dd/MM/yyyy", null),
                        CivicaCode = cashDump.FullText.Substring(53, 2).Trim(),
                        IsRead = false,
                        UPCashDumpId = cashDump.Id
                    })
                    .ToList();

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
    }

}
