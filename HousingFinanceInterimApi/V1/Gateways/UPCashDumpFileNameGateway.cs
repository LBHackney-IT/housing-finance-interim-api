using System;
using System.Linq;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The UP Cash file name gateway implementation.
    /// </summary>
    /// <seealso cref="IUPCashDumpFileNameGateway" />
    public class UPCashDumpFileNameGateway : IUPCashDumpFileNameGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCashDumpFileNameGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UPCashDumpFileNameGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the given file by the given file name asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// An instance of <see cref="UPCashDumpFileName" /> or null if no record found.
        /// </returns>
        public async Task<UPCashDumpFileName> GetAsync(string fileName)
            => await _context.UpCashDumpFileNames.FirstOrDefaultAsync(item => item.FileName.Equals(fileName) && item.IsSuccess.Equals(true))
                .ConfigureAwait(false);


        public async Task<UPCashDumpFileNameDomain> CreateAsync(string fileName, bool isSuccess = false)
        {
            try
            {
                var newUpCashDumpFileName = new UPCashDumpFileName()
                {
                    FileName = fileName,
                    IsSuccess = isSuccess
                };
                await _context.UpCashDumpFileNames.AddAsync(newUpCashDumpFileName).ConfigureAwait(false);

                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1
                    ? newUpCashDumpFileName.ToDomain()
                    : null;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Sets the given file name entry to success asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <returns>
        /// A bool determining the success of the method.
        /// </returns>
        public async Task<bool> SetToSuccessAsync(long fileId)
        {
            try
            {
                var upCashDumpFileName = await _context.UpCashDumpFileNames.FirstOrDefaultAsync(item => item.Id == fileId).ConfigureAwait(false);

                if (upCashDumpFileName == null)
                    return false;

                upCashDumpFileName.IsSuccess = true;
                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<UPCashDumpFileNameDomain> GetProcessedFileByName(string fileName)
        {
            try
            {
                var cashDumpFileName = _context.UpCashDumpFileNames.FirstOrDefault(item =>
                    item.FileName.Equals(fileName) &&
                    item.IsSuccess.Equals(true));

                return cashDumpFileName.ToDomain();
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
