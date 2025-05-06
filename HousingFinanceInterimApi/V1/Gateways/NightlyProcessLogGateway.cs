using Amazon.CloudWatchLogs.Model;
using HousingFinanceInterimApi.V1.Gateway.Interfaces;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.Handlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Gateways
{
    /// <summary>
    /// Gateway for parsing logs from AWS CloudWatch Logs Insights and updating the database with the results.
    /// </summary>
    /// <remarks>
    /// This gateway interacts with the database to log the results of queries executed against AWS CloudWatch Logs Insights.
    /// It processes log groups, extracts relevant information, and updates the database with success or failure entries.
    /// If there are any returned messages containing "error" it will save the Log Group name, the timestamp and the
    /// IsSuccess as False (failed) once, and move on to the next LogGroup Name.
    /// If no messages are returned by CW this signifies that there were no errors and the Log Group Name is saved with the
    /// isSuccess flag as True (succeeded).
    /// This is done to ensure that the Log Group status is saved once per call.
    /// </remarks>
    public class NightlyProcessLogGateway : INightlyProcessLogGateway
    {
        private readonly IDatabaseContext _context;

        public NightlyProcessLogGateway(IDatabaseContext context)
        {
            _context = context;
        }

        public async Task UpdateDatabaseWithResults(string logGroupName, List<List<ResultField>> queryResults)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(logGroupName))
            {
                throw new ArgumentNullException(nameof(logGroupName), "Log group name cannot be null or empty.");
            }

            // Handle null or empty queryResults
            if (queryResults is null)
            {
                // Case 3: No logs exist for the log group in the last 24 hours
                await LogResultAsync(logGroupName, null, null).ConfigureAwait(false);
                return;
            }

            if (queryResults.Count == 0)
            {
                // Case 2: Logs exist, but no match for the keyword
                await LogResultAsync(logGroupName, DateTime.UtcNow, true).ConfigureAwait(false);
                return;
            }

            try
            {
                foreach (var result in queryResults)
                {
                    var logEntry = ProcessResult(logGroupName, result);

                    // If a keyword is found, log it and stop processing further results
                    if (logEntry != null && logEntry.IsSuccess.HasValue && !logEntry.IsSuccess.Value)
                    {
                        await LogResultAsync(logEntry).ConfigureAwait(false);
                        return;
                    }
                }

                // If no keywords were found, log a success entry
                await LogResultAsync(logGroupName, DateTime.UtcNow, true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"Unexpected error in UpdateDatabaseWithResults for log group '{logGroupName}': {ex.Message}");
                LoggingHandler.LogError(ex.StackTrace);
                throw;
            }
        }

        private NightlyProcessLog ProcessResult(string logGroupName, List<ResultField> result)
        {
            try
            {
                var timestamp = result.Find(r => r.Field == "@timestamp")?.Value;
                var message = result.Find(r => r.Field == "@message")?.Value;

                if (DateTime.TryParse(timestamp, out var parsedTimestamp))
                {
                    // Check if the message contains the keyword
                    //TODO:: Dev-Testing with RequestId for now - revert to "ERROR" later
                    var isSuccess = message is not null && !message.Contains("requestid", StringComparison.OrdinalIgnoreCase);

                    return new NightlyProcessLog
                    {
                        LogGroupName = logGroupName,
                        Timestamp = parsedTimestamp,
                        IsSuccess = isSuccess,
                        DateCreated = DateTime.UtcNow
                    };
                }
                else
                {
                    LoggingHandler.LogError($"Invalid timestamp format in log group '{logGroupName}'. Result: {timestamp}");
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"Error processing result for log group '{logGroupName}': {ex.Message}");
                LoggingHandler.LogError(ex.StackTrace);
            }

            return null;
        }

        private async Task LogResultAsync(string logGroupName, DateTime? timestamp, bool? isSuccess)
        {
            var logEntry = new NightlyProcessLog
            {
                LogGroupName = logGroupName,
                Timestamp = timestamp,
                IsSuccess = isSuccess,
                DateCreated = DateTime.UtcNow
            };

            await LogResultAsync(logEntry).ConfigureAwait(false);
        }

        private async Task LogResultAsync(NightlyProcessLog logEntry)
        {
            try
            {
                await _context.NightlyProcessLogs.AddAsync(logEntry).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateException dbEx)
            {
                LoggingHandler.LogError($"Database update error for log group '{logEntry.LogGroupName}': {dbEx.Message}");
                LoggingHandler.LogError(dbEx.StackTrace);
                throw;
            }
        }

        public async Task<IList<NightlyProcessLog>> GetByDateCreatedAsync(DateTime createdDate)
        {
            return await _context.NightlyProcessLogs
                .Where(log => log.DateCreated.Date == createdDate.Date)
                .OrderByDescending(log => log.DateCreated)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
