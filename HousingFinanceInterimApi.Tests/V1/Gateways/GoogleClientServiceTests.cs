using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using HousingFinanceInterimApi.V1.Gateways;
using FluentAssertions;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System.Resources;
using HousingFinanceInterimApi.Tests.V1.Helper;
using Google.Apis.Drive.v3;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Gateways.Options;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

[assembly: NeutralResourcesLanguage("en")]

namespace HousingFinanceInterimApi.Tests.V1.Gateways
{
    //[Collection("AppTest collection")]
    public class GoogleSheetGatewayTests : IClassFixture<IntegrationTests<Startup>>
    {
        private GoogleClientService _classUnderTest;

        private readonly Mock<ILogger<GoogleClientService>> _logger;
        private readonly Mock<BaseClientService.Initializer> _initializer;

        public GoogleSheetGatewayTests()
        {
            _logger = new Mock<ILogger<GoogleClientService>>();
            _initializer = new Mock<BaseClientService.Initializer>();

            _classUnderTest = new GoogleClientService(_logger.Object, _initializer.Object);
        }
        //TODO: Fix test
        // [Fact]
        // public void ReadSheetToEntitiesAsyncProcessesValidCsvDataCorrectly()
        // {
        //     // Arrange
        //     const string spreadSheetId = "00999998";
        //     const string sheetName = "00999998";
        //     const string sheetRange = "00999998";
        //
        //     // Act
        //     var result = _classUnderTest.ReadSheetToEntitiesAsync<IList>(spreadSheetId, sheetName, sheetRange);
        //     // TestContext.Out.Write(
        //     //     JsonConvert.SerializeObject(result, Formatting.Indented, new StringEnumConverter()) +
        //     //     Environment.NewLine);
        //
        //     // Assert
        //     result.Should().NotBeNull();
        //     result.Result.Should().NotBeNull();
        // }
    }
}
