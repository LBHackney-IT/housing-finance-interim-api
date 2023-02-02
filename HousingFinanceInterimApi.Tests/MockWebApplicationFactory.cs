using System.Data.Common;
using CautionaryAlertsApi.Tests.V1.Helper;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using HousingFinanceInterimApi.Tests.V1.Factories;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace HousingFinanceInterimApi.Tests
{
    //[Collection("AppTest collection")]
    public class MockWebApplicationFactory<_TStartup>
        : WebApplicationFactory<_TStartup> where _TStartup : class
    {
        public SheetsService _sheetsService;
        public DbConnection _connection;

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
                var dbBuilder = new DbContextOptionsBuilder();
                dbBuilder.UseSqlServer(_connection);
                var context = new DatabaseContext(dbBuilder.Options);
                services.AddSingleton(context);

                var serviceProvider = services.BuildServiceProvider();
                var dbContext = serviceProvider.GetRequiredService<DatabaseContext>();

                dbContext.Database.EnsureCreated();
            });
            
            builder.ConfigureTestServices(services =>
            {
                var clientFactory = new FakeHttpClientFactory(new TestSpreadsheetHandler("test_cash_file.csv").RequestHandler);
                var baseClientService = new BaseClientService.Initializer { HttpClientFactory = clientFactory };

                _sheetsService = new SheetsService(baseClientService);

                services.RemoveAll<SheetsService>();
                services.AddScoped(provider => _sheetsService);
            });
        }
    }
}
