using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HousingFinanceInterimApi.Tests.V1.DatabaseContextFixtures
{
    public class SqlServerDockerContextFixture : IDatabaseContextFixture, IDisposable
    {
        HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
        private readonly List<Action> _cleanups;
        private readonly string _connectionString;
        private bool _disposed;

        public SqlServerDockerContextFixture(string connectionString)
        {
            _connectionString = connectionString;
            _cleanups = new List<Action>();
            _disposed = false;
        }

        public HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext>()
                .UseSqlServer(_connectionString)
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
                using var transaction = this._context.Database.BeginTransaction();
                try
                {
                    foreach (var cleanup in _cleanups)
                    {
                        cleanup();
                        this._context.SaveChanges();
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
                    this._context.Dispose();
                }
            }
            _disposed = true;
        }

        ~SqlServerDockerContextFixture()
        {
            Dispose(false);
        }
    }
}
