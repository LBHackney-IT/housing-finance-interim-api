module.exports = async (serverless) => {
    const resources = {};
    const stage = serverless.service.provider.stage;
    const service = serverless.service.service;
    
    Object.entries(serverless.service.functions).forEach(([funcKey, funcConfig]) => {
      const funcName = funcConfig.name || `${service}-${stage}-${funcKey}`;
      
      // Unique metric filter for each function
      resources[`${funcKey}TimeoutMetricFilter`] = {
        Type: "AWS::Logs::MetricFilter",
        Properties: {
          LogGroupName: `/aws/lambda/${funcName}`,
          FilterPattern: '"Task timed out after"',
          MetricTransformations: [{
            MetricValue: "1",
            MetricNamespace: "${self:service}-Timeouts",
            MetricName: `TimeoutCount-${funcKey}`,
            DefaultValue: 0
          }]
        }
      };

      // Individual alarm per function
      resources[`${funcKey}TimeoutAlarm`] = {
        Type: "AWS::CloudWatch::Alarm",
        Properties: {
          AlarmName: `${funcName}-timeout-alarm`,
          AlarmDescription: `Timeout detected in ${funcName}`,
          MetricName: `TimeoutCount-${funcKey}`,
          Namespace: "${self:service}-Timeouts",
          Statistic: "Sum",
          Period: 900,
          EvaluationPeriods: 1,
          Threshold: 0,
          ComparisonOperator: "GreaterThanThreshold",
          TreatMissingData: "notBreaching",
          AlarmActions: [{ Ref: "LambdaTimeoutAlarmTopic" }],
          Dimensions: [{
            Name: "FunctionName",
            Value: funcName
          }]
        }
      };
    });

    return resources;
}
