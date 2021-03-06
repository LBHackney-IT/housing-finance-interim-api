using System.Data.Common;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HousingFinanceInterimApi.Tests
{
    public class MockWebApplicationFactory<_TStartup>
        : WebApplicationFactory<_TStartup> where _TStartup : class
    {
        private readonly DbConnection _connection;

        public MockWebApplicationFactory(DbConnection connection)
        {
            _connection = connection;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .UseStartup<Startup>();
            builder.ConfigureServices(services =>
            {
                DbContextOptionsBuilder dbBuilder = new DbContextOptionsBuilder();
                dbBuilder.UseSqlServer(_connection);
                DatabaseContext context = new DatabaseContext(dbBuilder.Options);
                services.AddSingleton(context);

                ServiceProvider serviceProvider = services.BuildServiceProvider();
                DatabaseContext dbContext = serviceProvider.GetRequiredService<DatabaseContext>();

                dbContext.Database.EnsureCreated();
            });
        }
    }
}
