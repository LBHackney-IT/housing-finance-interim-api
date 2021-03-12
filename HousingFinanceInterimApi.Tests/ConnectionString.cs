using System;

namespace HousingFinanceInterimApi.Tests
{

    public static class ConnectionString
    {

        public static string TestDatabase()
        {
            return $"Server={Environment.GetEnvironmentVariable("DB_HOST") ?? "(localdb)\\mssqllocaldb"};" +
                   //$"User Id={Environment.GetEnvironmentVariable("DB_USERNAME") ?? "myuser"};" +
                   //$"Password={Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "mypassword"};" +
                   $"Database={Environment.GetEnvironmentVariable("DB_DATABASE") ?? "testdb"};" +
                   "Trusted_Connection=True;MultipleActiveResultSets=true";
        }

    }

}
