using AutoFixture;
using Bogus;
using HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext;
using HousingFinanceInterimApi.Tests.V1.spike289v2.DatabaseContextFixture;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using System;
using System.Collections.Generic;

namespace HousingFinanceInterimApi.Tests.V1.spike289v2.DatabaseContext
{
    public class DatabaseFixtureFactory
    {
        private readonly Faker _faker = new Faker();
        private readonly Fixture _fixture = new Fixture();
        private readonly List<Action> _cleanups = new List<Action>();

        public DatabaseFixtureFactory()
        {
        }

        public Faker Faker => _faker;

        public Fixture Fixture => _fixture;

        public List<Action> Cleanups => _cleanups;

        public IDatabaseContextFixture CreateFixture(string database)
        {
            IDatabaseContextFixture dbContextFixture;
            string connectionString;

            switch (database)
            {
                // Postgres RDS
                case ConstantsGen.POSTGRESRDS:
                    connectionString = "";
                    dbContextFixture = new PostgresRdsContextFixture(connectionString);
                    break;

                // SQL Server RDS
                case ConstantsGen.SQLSERVERRDS:
                    connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                    if (string.IsNullOrEmpty(connectionString))
                        throw new DbConnectionException("CONNECTION_STRING env var is not set");
                    dbContextFixture = new SqlServerRdsContextFixture(connectionString);
                    break;

                // SQL Server Docker
                case ConstantsGen.SQLSERVERDOCKER:
                    connectionString = "conn";
                    if (string.IsNullOrEmpty(connectionString))
                        throw new DbConnectionException("Docker connection string is not set");
                    dbContextFixture = new SqlServerDockerContextFixture(connectionString);
                    break;

                default:
                    connectionString = "";
                    dbContextFixture = new PostgresRdsContextFixture(connectionString);
                    break;
            }

            return dbContextFixture;
        }
    }
}
