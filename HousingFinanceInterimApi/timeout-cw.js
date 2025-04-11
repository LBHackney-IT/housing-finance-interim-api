// timeout-cw.js
module.exports = async ({ resolveVariable }) => {
    // Resolve core variables
    const service = await resolveVariable('self:service');
    const stage = await resolveVariable('sls:stage');
    const region = await resolveVariable('opt:region, self:provider.region, "us-east-1"');
    const functions = await resolveVariable('self:functions');
  
    const resources = {};
  
    // Generate resources for each function
    for (const [funcKey, funcConfig] of Object.entries(functions)) {
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
    }
  
    return resources;
};
