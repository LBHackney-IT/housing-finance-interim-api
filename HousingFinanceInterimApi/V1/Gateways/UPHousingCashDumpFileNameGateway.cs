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
    /// <seealso cref="IUPHousingCashDumpFileNameGateway" />
    public class UPHousingCashDumpFileNameGateway : IUPHousingCashDumpFileNameGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPHousingCashDumpFileNameGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UPHousingCashDumpFileNameGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the given file by the given file name asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// An instance of <see cref="UPHousingCashDumpFileName" /> or null if no record found.
        /// </returns>
        public async Task<UPHousingCashDumpFileName> GetAsync(string fileName)
            => await _context.UpHousingCashDumpFileNames.FirstOrDefaultAsync(item => item.FileName.Equals(fileName) && item.IsSuccess.Equals(true))
                .ConfigureAwait(false);

        /// <summary>
        /// Creates a UP Cash dump file name entry for the given file name asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
        /// <returns>
        /// The created instance of <see cref="UPHousingCashDumpFileName" />
        /// </returns>
        public async Task<UPHousingCashDumpFileNameDomain> CreateAsync(string fileName, bool isSuccess = false)
        {
            try
            {
                var newUpHousingCashDumpFileName = new UPHousingCashDumpFileName()
                {
                    FileName = fileName,
                    IsSuccess = isSuccess
                };
                await _context.UpHousingCashDumpFileNames.AddAsync(newUpHousingCashDumpFileName).ConfigureAwait(false);

                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1
                    ? newUpHousingCashDumpFileName.ToDomain()
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
                var upHousingCashDumpFileName = await _context.UpHousingCashDumpFileNames.FirstOrDefaultAsync(item => item.Id == fileId).ConfigureAwait(false);

                if (upHousingCashDumpFileName == null)
                    return false;

                upHousingCashDumpFileName.IsSuccess = true;
                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<UPHousingCashDumpFileNameDomain> GetProcessedFileByName(string fileName)
        {
            try
            {
                var housingCashDumpFileName = _context.UpHousingCashDumpFileNames.FirstOrDefault(item =>
                    item.FileName.Equals(fileName) &&
                    item.IsSuccess.Equals(true));

                return housingCashDumpFileName.ToDomain();
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
