// alarm-config.js
module.exports = (serverless) => {
    const { service, provider } = serverless.service;
    const stage = provider.stage;
    const accountId = provider.environment.AWS_ACCOUNT_ID;
    
    const resources = {};
  
    Object.entries(serverless.service.functions).forEach(([funcKey, funcConfig]) => {
      const funcName = funcConfig.name || `${service}-${stage}-${funcKey}`;
      const logicalIdPrefix = `${funcKey.replace(/-/g, '')}`;
  
      // Metric Filter Resource
      resources[`${logicalIdPrefix}TimeoutMetricFilter`] = {
        Type: 'AWS::Logs::MetricFilter',
        Properties: {
          LogGroupName: `/aws/lambda/${funcName}`,
          FilterPattern: '"Task timed out after"',
          MetricTransformations: [{
            MetricValue: '1',
            MetricNamespace: `${service}-Timeouts`,
            MetricName: `TimeoutCount-${funcKey}`,
            DefaultValue: 0
          }]
        }
      };
  
      // Alarm Resource
      resources[`${logicalIdPrefix}TimeoutAlarm`] = {
        Type: 'AWS::CloudWatch::Alarm',
        Properties: {
          AlarmName: `${funcName}-timeout-alarm`,
          AlarmDescription: `Timeout detected in ${funcName}`,
          MetricName: `TimeoutCount-${funcKey}`,
          Namespace: `${service}-Timeouts`,
          Statistic: 'Sum',
          Period: 900,
          EvaluationPeriods: 1,
          Threshold: 0,
          ComparisonOperator: 'GreaterThanThreshold',
          TreatMissingData: 'notBreaching',
          AlarmActions: [{ Ref: 'LambdaTimeoutAlarmTopic' }]
        }
      };
    });
  
    return resources;
  };