using System;
using Amazon.Lambda.Core;

namespace HousingFinanceInterimApi.V1.Handlers
{
    public static class LoggingHandler
    {
        public static void LogError(string message)
        {
            //throw new Exception($"[ERROR]: {message}");
            LambdaLogger.Log($"[ERROR]: {message}");
        }

        public static void LogWarning(string message)
        {
            LambdaLogger.Log($"[WARNING]: {message}");
        }

        public static void LogInfo(string message)
        {
            LambdaLogger.Log($"[INFO]: {message}");
        }
    }
}
