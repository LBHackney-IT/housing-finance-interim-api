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

            if (queryResults is null || queryResults.Count == 0)
            {
                throw new ArgumentNullException(nameof(queryResults), "Query results cannot be null or empty.");
            }

            try
            {
                NightlyProcessLog logEntry = null;

                foreach (var result in queryResults)
                {
                    try
                    {
                        var timestamp = result.Find(r => r.Field == "@timestamp")?.Value;
                        var message = result.Find(r => r.Field == "@message")?.Value;

                        if (DateTime.TryParse(timestamp, out var parsedTimestamp))
                        {
                            var isSuccess = message is not null
                                            && !message.Contains("error", StringComparison.OrdinalIgnoreCase);

                            // If a failure is found, immediately create the log entry and stop processing
                            if (!isSuccess)
                            {
                                logEntry = new NightlyProcessLog
                                {
                                    LogGroupName = logGroupName,
                                    Timestamp = parsedTimestamp,
                                    IsSuccess = false
                                };
                                break; // Stop processing further results
                            }

                            // If no failure is found, prepare a success entry (but don't add it yet)
                            if (logEntry == null)
                            {
                                logEntry = new NightlyProcessLog
                                {
                                    LogGroupName = logGroupName,
                                    Timestamp = parsedTimestamp,
                                    IsSuccess = true
                                };
                            }
                        }
                        else
                        {
                            LoggingHandler.LogError($"Invalid timestamp format in log group '{logGroupName}'. Result: {string.Join(", ", result.Select(r => $"{r.Field}: {r.Value}"))}");
                        }
                    }
                    catch (Exception innerEx)
                    {
                        LoggingHandler.LogError($"Error processing result for log group '{logGroupName}': {innerEx.Message}");
                        LoggingHandler.LogError(innerEx.StackTrace);
                    }
                }

                // Add the log entry to the database if one was created
                if (logEntry != null)
                {
                    // Add the log entry to the list and save
                    var processLogResults = new List<NightlyProcessLog> { logEntry }; 
                    await _context.NightlyProcessLogs.AddRangeAsync(processLogResults).ConfigureAwait(false);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (DbUpdateException dbEx)
            {
                LoggingHandler.LogError($"Database update error for log group '{logGroupName}': {dbEx.Message}");
                LoggingHandler.LogError(dbEx.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"Unexpected error in UpdateDatabaseWithResults for log group '{logGroupName}': {ex.Message}");
                LoggingHandler.LogError(ex.StackTrace);
                throw;
            }
        }
    }
}
