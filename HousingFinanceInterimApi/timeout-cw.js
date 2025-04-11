module.exports = async (serverless) => {
    // Gracefully handle missing context
    if (!serverless?.service?.provider) {
      return {}; // Return empty object instead of error
    }
  
    const { service } = serverless.service;
    const { stage, region } = serverless.service.provider;
    const accountId = serverless.providers?.aws?.getAccountId() || 'unknown';
  
    const resources = {};
  
    // Create resources only if valid functions exist
    if (serverless.service.functions) {
      Object.entries(serverless.service.functions).forEach(([funcKey, funcConfig]) => {
        const funcName = funcConfig.name || `${service}-${stage}-${funcKey}`;
        const safeKey = funcKey.replace(/[^a-zA-Z0-9]/g, '');
  
        // Metric Filter
        resources[`${safeKey}TimeoutMetric`] = {
          Type: 'AWS::Logs::MetricFilter',
          Properties: {
            LogGroupName: `/aws/lambda/${funcName}`,
            FilterPattern: '"Task timed out after"',
            MetricTransformations: [{
              MetricValue: '1',
              MetricNamespace: `${service}-Timeouts`,
              MetricName: `TimeoutCount-${safeKey}`,
              DefaultValue: 0
            }]
          }
        };
  
        // CloudWatch Alarm
        resources[`${safeKey}TimeoutAlarm`] = {
          Type: 'AWS::CloudWatch::Alarm',
          Properties: {
            AlarmName: `${funcName}-timeout-alarm`,
            AlarmDescription: `Timeout in ${funcName} (${stage})`,
            MetricName: `TimeoutCount-${safeKey}`,
            Namespace: `${service}-Timeouts`,
            Statistic: 'Sum',
            Period: 300,
            EvaluationPeriods: 1,
            Threshold: 0,
            ComparisonOperator: 'GreaterThanThreshold',
            TreatMissingData: 'notBreaching',
            AlarmActions: [{ Ref: 'LambdaTimeoutAlarmTopic' }],
            Dimensions: [{
              Name: 'FunctionName',
              Value: funcName
            }]
          }
        };
      });
    }
  
    return resources;
};
