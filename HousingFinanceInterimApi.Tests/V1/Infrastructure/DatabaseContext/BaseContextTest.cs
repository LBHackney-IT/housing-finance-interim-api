using AutoFixture;
using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext
{
    public class DbConnectionException : Exception { public DbConnectionException(string message) : base(message) { } }

    public class BaseContextTest : IDisposable
    {
        internal readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
        internal readonly Fixture _fixture;
        internal readonly Faker _faker;
        internal List<Action> _cleanups;
        private bool _disposed;

        public BaseContextTest()
        {
            _context = CreateDbContext();
            _fixture = new Fixture();
            _faker = new Faker();
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
            // TODO: Implement a way to switch between SQL Server and Postgres
            var Server = "127.0.0.1"; // TODO: Should fetch from env var when it has Docker support
            var connectionString = $"Server={Server};Database=sow2b;User Id=sa;Password=password123!;";

            var options = new DbContextOptionsBuilder<HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext>()
                .UseSqlServer(connectionString)
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
                    {
                        cleanup();
                        _context.SaveChanges();
                    }
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
}
