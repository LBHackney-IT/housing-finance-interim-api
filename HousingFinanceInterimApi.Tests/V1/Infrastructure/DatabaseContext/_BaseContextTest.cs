using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;

namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "This is a test class")]

public class DbConnectionException : Exception { public DbConnectionException(string message) : base(message) { } }

public class BaseContextTest : IDisposable
{
    internal readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
    internal readonly Fixture _fixture;
    internal List<Action> _cleanups;
    private bool _disposed;

    public BaseContextTest()
    {
        _context = CreateDbContext();
        _fixture = new Fixture();
        _cleanups = new List<Action>();
    }

    public void ExecuteProcedure(string procedureName)
    {
        using var transaction = _context.Database.BeginTransaction();
        _context.Database.SetCommandTimeout(900);
        _context.Database.ExecuteSqlRaw(procedureName);
        transaction.Commit();
    }

    private static HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext CreateDbContext()
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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                foreach (var cleanup in _cleanups)
                    cleanup();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    ~BaseContextTest()
    {
        Dispose(false);
    }
}
