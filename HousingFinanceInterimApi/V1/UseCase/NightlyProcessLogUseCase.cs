using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateway.Interfaces;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase
{
    /// <summary>
    /// Use case for parsing logs from AWS CloudWatch Logs Insights and updating the database with the results.
    /// </summary>
    /// <remarks>
    /// This use case processes a list of log groups, queries CloudWatch Logs Insights for each log group,
    /// and updates the database with the query results. It handles errors during the querying and database update
    /// processes, logging failures to the database when necessary.
    /// </remarks>
    public class NightlyProcessLogUseCase : INightlyProcessLogUseCase
    {
        private readonly INightlyProcessLogGateway _nightlyprocessLogGateway;
        private readonly IAmazonCloudWatchLogs _cloudWatchLogsClient;
        private readonly IList<string> _logGroups;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION") ?? "100";

        public NightlyProcessLogUseCase(
            INightlyProcessLogGateway nightlyprocessLogGateway,
            IAmazonCloudWatchLogs cloudWatchLogsClient,
            IList<string> logGroups)
        {
            _nightlyprocessLogGateway = nightlyprocessLogGateway;
            _cloudWatchLogsClient = cloudWatchLogsClient;
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
                foreach (var logGroup in _logGroups)
                {
                    try
                    {
                        var queryResults = await QueryCloudWatchLogs(logGroup).ConfigureAwait(false);
                        await _nightlyprocessLogGateway.UpdateDatabaseWithResults(logGroup, queryResults).ConfigureAwait(false);
                    }
                    catch (AmazonCloudWatchLogsException awsEx)
                    {
                        LoggingHandler.LogError($"AWS error for log group {logGroup}: {awsEx.Message}");
                        await LogFailureToDatabase(logGroup, awsEx.Message).ConfigureAwait(false);
                    }
                    catch (DbUpdateException dbEx)
                    {
                        LoggingHandler.LogError($"Database update error for log group {logGroup}: {dbEx.Message}");
                        throw;
                    }
                    catch (System.InvalidOperationException invalidOpEx)
                    {
                        LoggingHandler.LogError($"Invalid operation for log group {logGroup}: {invalidOpEx.Message}");
                        await LogFailureToDatabase(logGroup, invalidOpEx.Message).ConfigureAwait(false);
                    }
                    catch (Exception ex) when (ex is AmazonCloudWatchLogsException || ex is DbUpdateException || ex is System.InvalidOperationException)
                    {
                        // Catch any other unexpected exceptions
                        LoggingHandler.LogError($"Unexpected error for log group {logGroup}: {ex.Message}");
                        await LogFailureToDatabase(logGroup, ex.Message).ConfigureAwait(false);
                    }
                }

                return new StepResponse() { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"Error in Log Parser Lambda: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Case 1: Results exist for the keyword - return the results list
        /// Case 2: Logs exist, but no match for the keyword - return an empty list
        /// Case 3: No logs exist for the log group in the last 24 hours - return null
        /// </summary>
        /// <param name="logGroupName"></param>
        /// <returns></returns>
        public async Task<List<List<ResultField>>> QueryCloudWatchLogs(string logGroupName)
        {
            try
            {
                // First query: Check for the keyword
                var keywordQuery = @"
                fields @timestamp, @message
                | filter @message like /error/
                | sort @timestamp desc
                | limit 100";

                var startQueryRequest = new StartQueryRequest
                {
                    LogGroupName = logGroupName,
                    StartTime = new DateTimeOffset(DateTime.UtcNow.AddDays(-1)).ToUnixTimeMilliseconds(),
                    EndTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds(),
                    QueryString = keywordQuery
                };

                var startQueryResponse = await _cloudWatchLogsClient.StartQueryAsync(startQueryRequest).ConfigureAwait(false);

                // Wait for the query to complete
                var queryResults = await GetQueryResultsAsync(startQueryResponse.QueryId).ConfigureAwait(false);

                if (queryResults.Count != 0)
                {
                    // Case 1: Results found for the keyword
                    return queryResults;
                }

                // Baseline query: Check if logs exist at all
                var baselineQuery = @"
                fields @timestamp
                | sort @timestamp desc
                | limit 1";

                startQueryRequest.QueryString = baselineQuery;
                startQueryResponse = await _cloudWatchLogsClient.StartQueryAsync(startQueryRequest).ConfigureAwait(false);

                var baselineResults = await GetQueryResultsAsync(startQueryResponse.QueryId).ConfigureAwait(false);

                if (baselineResults.Count == 0)
                {
                    // Case 3: No logs exist for the log group in the last 24 hours
                    return null;
                }

                // Case 2: Logs exist, but no match for the keyword
                return new List<List<ResultField>>();
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

        private async Task<List<List<ResultField>>> GetQueryResultsAsync(string queryId)
        {
            GetQueryResultsResponse queryResultsResponse;
            do
            {
                await Task.Delay(1000).ConfigureAwait(false); // Wait for 1 second before polling
                queryResultsResponse = await _cloudWatchLogsClient.GetQueryResultsAsync(new GetQueryResultsRequest { QueryId = queryId }).ConfigureAwait(false);

                if (queryResultsResponse.Status == QueryStatus.Failed)
                {
                    throw new AmazonCloudWatchLogsException($"CloudWatch Insights Query failed. Status: {queryResultsResponse.Status}");
                }
            } while (queryResultsResponse.Status == QueryStatus.Running || queryResultsResponse.Status == QueryStatus.Scheduled);

            return queryResultsResponse.Results;
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
                await _nightlyprocessLogGateway.UpdateDatabaseWithResults(logGroup, failedQueryResult).ConfigureAwait(false);
            }
            catch (Exception dbEx)
            {
                LoggingHandler.LogError($"Failed to log query failure to database for log group {logGroup}: {dbEx.Message}");
                throw;
            }
        }

        #region Controller

        public async Task<IList<NightlyProcessLogResponse>> ExecuteAsync(DateTime createdDate)
        {
            if (createdDate == default)
            {
                throw new ArgumentException("The createdDate parameter cannot be the default value.", nameof(createdDate));
            }

            try
            {
                var logs = await _nightlyprocessLogGateway.GetByDateCreatedAsync(createdDate).ConfigureAwait(false);

                if (logs == null)
                {
                    LoggingHandler.LogWarning($"No logs found for the provided date: {createdDate:yyyy-MM-dd}");
                    return new List<NightlyProcessLogResponse>();
                }

                return logs.Select(log => new NightlyProcessLogResponse
                {
                    Id = log.Id,
                    LogGroupName = log.LogGroupName,
                    Timestamp = log.Timestamp,
                    IsSuccess = log.IsSuccess,
                    DateCreated = log.DateCreated
                }).ToList();
            }
            catch (DbUpdateException dbEx)
            {
                LoggingHandler.LogError($"Database error while retrieving logs for date {createdDate:yyyy-MM-dd}: {dbEx.Message}");
                throw new System.InvalidOperationException("An error occurred while accessing the database.", dbEx);
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"Unexpected error while retrieving logs for date {createdDate:yyyy-MM-dd}: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred while processing the request.", ex);
            }
        }


        #endregion
    }
}
