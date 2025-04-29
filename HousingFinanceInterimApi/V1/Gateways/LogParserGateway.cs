using Amazon.CloudWatchLogs.Model;
using HousingFinanceInterimApi.V1.Gateway.Interfaces;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.Handlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            try
            {
                foreach (var result in queryResults)
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

                        // Add the single row to the database
                        await _context.NightlyProcessLogs.AddAsync(processLogResult).ConfigureAwait(false);
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
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
