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
    public class LogParserGateway : ILogParserGateway
    {
        private readonly IDatabaseContext _context;

        public LogParserGateway(IDatabaseContext context)
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
            if (queryResults is null || !queryResults.Any())
            {
                await LogResultAsync(logGroupName, DateTime.UtcNow, true).ConfigureAwait(false);
                return;
            }

            try
            {
                foreach (var result in queryResults)
                {
                    var logEntry = ProcessResult(logGroupName, result);

                    // If a failure is found, log it and stop processing further results
                    if (logEntry != null && !logEntry.IsSuccess)
                    {
                        await LogResultAsync(logEntry).ConfigureAwait(false);
                        return;
                    }
                }

                // If no failures were found, log a success entry
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
                    var isSuccess = message is not null && !message.Contains("error", StringComparison.OrdinalIgnoreCase);

                    return new NightlyProcessLog
                    {
                        LogGroupName = logGroupName,
                        Timestamp = parsedTimestamp,
                        IsSuccess = isSuccess
                    };
                }
                else
                {
                    LoggingHandler.LogError($"Invalid timestamp format in log group '{logGroupName}'. Result: {string.Join(", ", result.Select(r => $"{r.Field}: {r.Value}"))}");
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"Error processing result for log group '{logGroupName}': {ex.Message}");
                LoggingHandler.LogError(ex.StackTrace);
            }

            return null;
        }

        private async Task LogResultAsync(string logGroupName, DateTime timestamp, bool isSuccess)
        {
            var logEntry = new NightlyProcessLog
            {
                LogGroupName = logGroupName,
                Timestamp = timestamp,
                IsSuccess = isSuccess
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
    }
}
