using System;
using System.Collections.Generic;
using NUnit.Framework;
using HousingFinanceInterimApi.V1.Gateways;
using FluentAssertions;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System.Resources;
using CautionaryAlertsApi.Tests.V1.Factories;
using CautionaryAlertsApi.Tests.V1.Helper;
using Google.Apis.Drive.v3;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Gateways.Options;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

[assembly: NeutralResourcesLanguage("en")]

namespace CautionaryAlertsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class GoogleSheetGatewayTests
    {
        private SheetsService _sheetsService;
        private GoogleClientService _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            var clientFactory = new FakeHttpClientFactory(new TestSpreadsheetHandler("test_cash_file.csv").RequestHandler);
            var baseClientService = new BaseClientService.Initializer { HttpClientFactory = clientFactory };


            var options = Options.Create(new GoogleClientServiceOptions
            {
                ApplicationName = "Hackney Finance Interim Solution",
                Scopes = new List<string>
                {
                    DriveService.Scope.Drive, SheetsService.Scope.SpreadsheetsReadonly
                }
            });

            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
            DatabaseContext context = new DatabaseContext(optionsBuilder.Options);

            IGoogleClientService googleClientService =
                new GoogleClientServiceFactory(default, options, context)
                    .CreateGoogleClientServiceFromJson(Environment.GetEnvironmentVariable("GOOGLE_API_KEY"));

            GoogleClientService _classUnderTest = (GoogleClientService) googleClientService;

            _sheetsService = new SheetsService(baseClientService);
            _classUnderTest._sheetsService = _sheetsService;
        }

        // [Test]
        // public void GetsCautionaryAlertListItemForPropertyReference()
        // {
        //     // Arrange
        //     const string spreadSheetId = "00999998";
        //     const string sheetName = "00999998";
        //     const string sheetRange = "00999998";
        //
        //     // Act
        //     var result = _classUnderTest.ReadSheetToEntitiesAsync<IList<IList>>(spreadSheetId, sheetName, sheetRange);
        //     TestContext.Out.Write(
        //         JsonConvert.SerializeObject(result, Formatting.Indented, new StringEnumConverter()) +
        //         Environment.NewLine);
        //
        //     // Assert
        //     result.Count.Should().Be(2);
        //     result.Should().ContainSingle(alert => alert.Address == "Fake Place 4");
        //     result.Should().ContainSingle(alert => alert.Address == "Fake Place 5");
        //     result.Should().OnlyContain(alert => alert.PropertyReference == propertyReference);
        // }
        //
        // [Test]
        // public void GetsCautionaryAlertListItemForPersonId()
        // {
        //     // Arrange
        //     const string personId = "566c45c2-1f0c-4ecf-8fbf-afe62d51c8ba";
        //
        //     // Act
        //     var result = _classUnderTest.GetPersonAlerts(personId).ToList();
        //
        //     TestContext.Out.Write(
        //         JsonConvert.SerializeObject(result, Formatting.Indented, new StringEnumConverter()) +
        //         Environment.NewLine);
        //
        //     // Assert
        //     result.Should().ContainSingle(alert => alert.CautionOnSystem == "Caution Type 2" && alert.Outcome == "Caution Description 2");
        //     result.Should().ContainSingle(alert => alert.CautionOnSystem == "Caution Type 5" && alert.Outcome == "Caution Description 5");
        // }
    }
}
