using Amazon.CloudWatchLogs.Model;
using HousingFinanceInterimApi.V1.Gateway.Interfaces;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.Handlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Domain;

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
                        var processLogResult = new ProcessLog
                        {
                            LogGroupName = logGroupName,
                            Timestamp = parsedTimestamp,
                            //TODO:: Add contains for the correct string to determine success or failure
                            IsSuccess = message != null && message.Contains("success", StringComparison.OrdinalIgnoreCase)
                        };

                        // Add the single row to the database
                        await _context.ProcessLogs.AddAsync(processLogResult).ConfigureAwait(false);
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
