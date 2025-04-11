// timeout-cw.js
module.exports = (serverless) => {
    // Validate serverless context exists
    if (!serverless || !serverless.service) {
      throw new Error('Serverless context not available');
    }
  
    const { service } = serverless.service;
    const { stage, region } = serverless.service.provider;
    const resources = {};
  
    Object.entries(serverless.service.functions).forEach(([funcKey, funcConfig]) => {
      const funcName = funcConfig.name || `${service}-${stage}-${funcKey}`;
      const logicalId = `${funcKey.replace(/[^a-zA-Z0-9]/g, '')}TimeoutAlarm`;
  
      // Metric Filter
      resources[`${logicalId}MetricFilter`] = {
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
  
      // CloudWatch Alarm
      resources[logicalId] = {
        Type: 'AWS::CloudWatch::Alarm',
        Properties: {
          AlarmName: `${funcName}-timeout-alarm`,
          AlarmDescription: `Timeout detected in ${funcName} (${region})`,
          MetricName: `TimeoutCount-${funcKey}`,
          Namespace: `${service}-Timeouts`,
          Statistic: 'Sum',
          Period: 300,
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
