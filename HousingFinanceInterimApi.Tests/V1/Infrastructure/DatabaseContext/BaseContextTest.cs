using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext.TestFactories;


namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "This is a test class")]


public class BaseContextTest : IDisposable
{
    internal readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext Context;
    internal readonly Fixture Fixture;
    internal List<Action> Cleanups;
    private bool _disposed;

    public BaseContextTest()
    {
        Context = HfsDbContextFactory.Create();
        Fixture = new Fixture();
        Cleanups = new List<Action>();
    }

    public async Task ExecuteProcedure(string procedureName)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync().ConfigureAwait(false);
        Context.Database.SetCommandTimeout(900);
        await Context.Database.ExecuteSqlRawAsync(procedureName).ConfigureAwait(false);
        await transaction.CommitAsync().ConfigureAwait(false);
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
            using var transaction = Context.Database.BeginTransaction();
            try
            {
                foreach (var cleanup in Cleanups)
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
                Context.Dispose();
            }
        }
        _disposed = true;
    }

    ~BaseContextTest()
    {
        Dispose(false);
    }
}
