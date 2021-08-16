using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class BatchLogErrorGateway : IBatchLogErrorGateway
    {
        private readonly DatabaseContext _context;

        public BatchLogErrorGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<BatchLogError> CreateAsync(long batchId, string type, string message)
        {
            var newBatchError = new BatchLogError()
            {
                Type = type,
                BatchLogId = batchId,
                Message = message
            };
            await _context.BatchLogErrors.AddAsync(newBatchError).ConfigureAwait(false);

            return await _context.SaveChangesAsync().ConfigureAwait(false) == 1
                    ? newBatchError
                    : null;
        }
    }
}
