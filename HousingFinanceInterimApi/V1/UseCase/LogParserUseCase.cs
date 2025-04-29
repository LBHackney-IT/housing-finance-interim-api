using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Gateway.Interfaces;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Microsoft.EntityFrameworkCore;
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
        private readonly IList<string> _logGroups;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION") ?? "100";

        public LogParserUseCase(
            ILogParserGateway logParserGateway,
            IAmazonCloudWatchLogs cloudWatchLogsClient,
            IList<string> logGroups)
        {
            _cloudWatchLogsClient = cloudWatchLogsClient;
            _logParserGateway = logParserGateway;
            _logGroups = logGroups ?? throw new ArgumentNullException(nameof(logGroups));
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            if (!_logGroups.Any())
            {
                throw new ArgumentException("Log groups cannot be null or empty", nameof(_logGroups));
            }

            try
            {
                // Process log groups in parallel
                var tasks = _logGroups.Select(async logGroup =>
                {
                    try
                    {
                        // Query CloudWatch Logs Insights
                        var queryResults = await QueryCloudWatchLogs(logGroup).ConfigureAwait(false);

                        // Evaluate results and update the database
                        await _logParserGateway.UpdateDatabaseWithResults(logGroup, queryResults).ConfigureAwait(false);
                    }
                    catch (AmazonCloudWatchLogsException awsEx)
                    {
                        // Handle AWS-specific errors
                        LoggingHandler.LogError($"AWS error for log group {logGroup}: {awsEx.Message}");

                        await LogFailureToDatabase(logGroup, awsEx.Message).ConfigureAwait(false);
                    }
                    catch (DbUpdateException dbEx)
                    {
                        // Handle database-specific errors
                        LoggingHandler.LogError($"Database update error for log group {logGroup}: {dbEx.Message}");
                        throw; 
                    }
                    catch (Exception ex)
                    {
                        // Handle other unexpected errors
                        LoggingHandler.LogError($"Unexpected error for log group {logGroup}: {ex.Message}");

                        await LogFailureToDatabase(logGroup, ex.Message).ConfigureAwait(false);
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

        public async Task<List<List<ResultField>>> QueryCloudWatchLogs(string logGroupName)
        {
            try
            {
                // TODO:: Dev-Testing with RequestId for now - revert to "ERROR" later
                var query = @"
                        fields @timestamp, @message
                        | filter @message like /RequestId/
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
                    await Task.Delay(1000).ConfigureAwait(false); // Wait for 1 second before polling
                    queryResultsResponse = await _cloudWatchLogsClient.GetQueryResultsAsync(getQueryResultsRequest).ConfigureAwait(false);

                    if (queryResultsResponse.Status == QueryStatus.Failed)
                    {
                        throw new AmazonCloudWatchLogsException($"CloudWatch Insights Query failed. Status: {queryResultsResponse.Status}");
                    }
                } while (queryResultsResponse.Status == QueryStatus.Running || queryResultsResponse.Status == QueryStatus.Scheduled);

                return queryResultsResponse.Results;
            }
            catch (AmazonCloudWatchLogsException awsEx)
            {
                LoggingHandler.LogError($"AWS CloudWatch Logs error for log group {logGroupName}: {awsEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"Unexpected error while querying CloudWatch Logs for log group {logGroupName}: {ex.Message}");
                throw;
            }
        }
        private async Task LogFailureToDatabase(string logGroup, string errorMessage)
        {
            var failedQueryResult = new List<List<ResultField>>
            {
                new List<ResultField>
                {
                    new ResultField { Field = "Error", Value = errorMessage },
                    new ResultField { Field = "LogGroupName", Value = logGroup }
                }
            };

            try
            {
                await _logParserGateway.UpdateDatabaseWithResults(logGroup, failedQueryResult).ConfigureAwait(false);
            }
            catch (Exception dbEx)
            {
                LoggingHandler.LogError($"Failed to log query failure to database for log group {logGroup}: {dbEx.Message}");
            }
        }
    }
}
