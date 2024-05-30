
using System;
using Microsoft.EntityFrameworkCore;

namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext.TestFactories;

public class DbConnectionException : Exception { public DbConnectionException(string message) : base(message) { } }

public static class HfsDbContextFactory
{
    public static HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext Create()
    {
        var connectionStringEnvVar = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionStringEnvVar))
            throw new DbConnectionException("CONNECTION_STRING env var is not set");

        var options = new DbContextOptionsBuilder<HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext>()
            .UseSqlServer(connectionStringEnvVar)
            .Options;

        var context = new HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext(options);
        context.Database.OpenConnection();

        return context;
    }
}
