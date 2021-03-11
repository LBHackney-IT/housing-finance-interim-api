using System.Net.Http;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;

namespace HousingFinanceInterimApi.Tests
{

    public class IntegrationTests<_TStartup> where _TStartup : class
    {

        protected HttpClient Client { get; private set; }
        protected DatabaseContext DatabaseContext { get; private set; }

        private MockWebApplicationFactory<_TStartup> _factory;
        private SqlConnection _connection;
        private IDbContextTransaction _transaction;
        private DbContextOptionsBuilder _builder;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _connection = new SqlConnection(ConnectionString.TestDatabase());
            _connection.Open();
            SqlCommand npgsqlCommand = _connection.CreateCommand();
            npgsqlCommand.CommandText = "SET deadlock_timeout TO 30";
            npgsqlCommand.ExecuteNonQuery();

            _builder = new DbContextOptionsBuilder();
            _builder.UseSqlServer(_connection);
        }

        [SetUp]
        public void BaseSetup()
        {
            _factory = new MockWebApplicationFactory<_TStartup>(_connection);
            Client = _factory.CreateClient();
            DatabaseContext = new DatabaseContext(_builder.Options);
            DatabaseContext.Database.EnsureCreated();
            _transaction = DatabaseContext.Database.BeginTransaction();
        }

        [TearDown]
        public void BaseTearDown()
        {
            Client.Dispose();
            _factory.Dispose();
            _transaction.Rollback();
            _transaction.Dispose();
        }

    }

}
