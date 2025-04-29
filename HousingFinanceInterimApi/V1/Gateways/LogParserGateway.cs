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
        private readonly DatabaseContext _context;

        public LogParserGateway(DatabaseContext context)
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

            if (queryResults == null || !queryResults.Any())
            {
                throw new ArgumentNullException(nameof(queryResults), "Query results cannot be null or empty.");
            }

            try
            {
                var processLogResults = new List<NightlyProcessLog>();

                foreach (var result in queryResults)
                {
                    try
                    {
                        var timestamp = result.Find(r => r.Field == "@timestamp")?.Value;
                        var message = result.Find(r => r.Field == "@message")?.Value;

                        if (DateTime.TryParse(timestamp, out var parsedTimestamp))
                        {
                            var processLogResult = new NightlyProcessLog
                            {
                                LogGroupName = logGroupName,
                                Timestamp = parsedTimestamp,
                                IsSuccess = message is not null
                                            && message.Contains("ERROR", StringComparison.OrdinalIgnoreCase) is not true
                            };

                            processLogResults.Add(processLogResult);
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

                // Batch insert all results into the database
                if (processLogResults.Any())
                {
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
