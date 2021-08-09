using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    /// <summary>
    /// The UP Cash file name gateway implementation.
    /// </summary>
    /// <seealso cref="IUPHousingCashFileNameGateway" />
    public class UPHousingCashFileNameGateway : IUPHousingCashFileNameGateway
    {

        /// <summary>
        /// The database context
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPHousingCashFileNameGateway"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UPHousingCashFileNameGateway(DatabaseContext context)
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
        public async Task<UPHousingCashDumpFileName> CreateAsync(string fileName, bool isSuccess = false)
        {
            var getResult = await GetAsync(fileName).ConfigureAwait(false);

            if (getResult == null)
            {
                // Create and add
                UPHousingCashDumpFileName newFileName = new UPHousingCashDumpFileName
                {
                    FileName = fileName,
                    IsSuccess = isSuccess
                };
                await _context.UpHousingCashDumpFileNames.AddAsync(newFileName).ConfigureAwait(false);

                // If saved successfully, return entity
                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1
                    ? newFileName
                    : null;
            }

            return null;
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
            var fileName = await _context.UpHousingCashDumpFileNames.FirstOrDefaultAsync(item => item.Id == fileId)
                .ConfigureAwait(false);

            if (fileName != null)
            {
                fileName.IsSuccess = true;

                return await _context.SaveChangesAsync().ConfigureAwait(false) == 1;
            }

            return false;
        }

    }

}
