using System;

namespace HousingFinanceInterimApi.Tests
{

    public static class ConnectionString
    {

        public static string TestDatabase()
        {
            return $"User Id={Environment.GetEnvironmentVariable("MSSQL_PID") ?? "Enterprise"};" +
                   $"Password={Environment.GetEnvironmentVariable("SA_PASSWORD") ?? "mypassword"};" +
                   "Trusted_Connection=True;MultipleActiveResultSets=true";
        }

    }

}
