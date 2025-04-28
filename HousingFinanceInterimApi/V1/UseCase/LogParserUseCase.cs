using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Gateway.Interfaces;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class LogParserUseCase : ILogParserUseCase
    {
        private readonly IAmazonCloudWatchLogs _cloudWatchLogsClient;
        private readonly ILogParserGateway _logParserGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        public LogParserUseCase(ILogParserGateway logParserGateway)
        {
            _cloudWatchLogsClient = new AmazonCloudWatchLogsClient();
            _logParserGateway = logParserGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            try
            {
                // TODO:: Define all the log groups to query
                var logGroups = new List<string> {
                        "/aws/lambda/Function1",
                        "/aws/lambda/Function2"
                    };

                // Process log groups in parallel
                var tasks = logGroups.Select(async logGroup =>
                {
                    try
                    {
                        // Query CloudWatch Logs Insights
                        var queryResults = await QueryCloudWatchLogs(logGroup).ConfigureAwait(false);

                        // Evaluate results and update the database
                        await _logParserGateway.UpdateDatabaseWithResults(logGroup, queryResults).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        // Log the failure in the database
                        var failedQueryResult = new List<List<ResultField>>
                        {
                                new List<ResultField>
                                {
                                    new ResultField { Field = "Error", Value = ex.Message },
                                    new ResultField { Field = "LogGroupName", Value = logGroup }
                                }
                        };

                        try
                        {
                            // Log the failure in the database
                            await _logParserGateway.UpdateDatabaseWithResults(logGroup, failedQueryResult).ConfigureAwait(false);
                        }
                        catch (Exception dbEx)
                        {
                            // Log database errors to the logging system
                            LoggingHandler.LogError($"Failed to log query failure to database: {dbEx.Message}");
                        }

                        // Log the error to the logging system
                        LoggingHandler.LogError($"Query failed for log group {logGroup}: {ex.Message}");
                    }
                });

                // Wait for all tasks to complete
                await Task.WhenAll(tasks).ConfigureAwait(false);

                return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"Error in Log Parser Lambda: {ex.Message}");
                throw;
            }
        }

        private async Task<List<List<ResultField>>> QueryCloudWatchLogs(string logGroupName)
        {
            //TODO: Fix query with the correct syntax
            var query = @"
                    fields @timestamp, @message
                    | filter @message like /success|failure/
                    | sort @timestamp desc
                    | limit 100";

            var startQueryRequest = new StartQueryRequest
            {
                LogGroupName = logGroupName,
                StartTime = new DateTimeOffset(DateTime.UtcNow.AddDays(-1)).ToUnixTimeMilliseconds(),
                EndTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds(),
                QueryString = query
            };

            var startQueryResponse = await _cloudWatchLogsClient.StartQueryAsync(startQueryRequest).ConfigureAwait(false);

            // Wait for the query to complete
            var getQueryResultsRequest = new GetQueryResultsRequest
            {
                QueryId = startQueryResponse.QueryId
            };

            GetQueryResultsResponse queryResultsResponse;
            do
            {
                await Task.Delay(2000).ConfigureAwait(false); // Wait for 2 seconds before polling
                queryResultsResponse = await _cloudWatchLogsClient.GetQueryResultsAsync(getQueryResultsRequest).ConfigureAwait(false);
                if (queryResultsResponse.Status == QueryStatus.Failed)
                {
                    throw new Exception($"Query failed: {queryResultsResponse.Status}");
                }
            } while (queryResultsResponse.Status == QueryStatus.Running);

            return queryResultsResponse.Results;
        }
    }
}
