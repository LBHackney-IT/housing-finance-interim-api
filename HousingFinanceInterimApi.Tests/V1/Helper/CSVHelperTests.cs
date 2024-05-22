using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Domain.ArgumentWrappers;
using HousingFinanceInterimApi.V1.Helpers;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Helpers
{
    public class CSVHelperTests
    {
        [Fact]
        public void CSVHelperCreatesProperCSVStringFromTheCollection()
        {
            // arrange
            var smallEasyToTestWithModels = RandomGen.CreateMany<GetPRNTransactionsDomain>(quantity: 2);

            var firstItem = smallEasyToTestWithModels[0];
            var secondItem = smallEasyToTestWithModels[1];

            secondItem.RentGroup = string.Empty;

            var expectedCSVString = "RentGroup,FinancialYear,StartWeekOrMonth,EndWeekOrMonth"
                + $"\n{firstItem.RentGroup},{firstItem.FinancialYear},{firstItem.StartWeekOrMonth},{firstItem.EndWeekOrMonth}"
                + $"\n{secondItem.RentGroup},{secondItem.FinancialYear},{secondItem.StartWeekOrMonth},{secondItem.EndWeekOrMonth}";

            // act
            var csvStringOutput = CSVHelper.ToCSVString(smallEasyToTestWithModels);

            // assert
            csvStringOutput.Should().Be(expectedCSVString);
        }

        [Fact]
        public void CSVHelperCreatesCSVStringWithHeadersOnlyWhenTheCollectionIsEmpty()
        {
            // arrange
            var emptyModelCollection = new List<GetPRNTransactionsDomain>();

            var expectedCSVString = "RentGroup,FinancialYear,StartWeekOrMonth,EndWeekOrMonth";

            // act
            var csvStringOutput = CSVHelper.ToCSVString(emptyModelCollection);

            // assert
            csvStringOutput.Should().Be(expectedCSVString);
        }

        [Fact]
        public void CSVHelperCreatesCorrectMemoryStreamBasedOnTheInputCSVString()
        {
            // arrange
            var inputCSVString = "RentGroup,FinancialYear,StartWeekOrMonth,EndWeekOrMonth" +
                "\nHRA,2023,1,52" +
                "\nLMW,2021,7,12" +
                "\n,2025,2,5";

            // act
            var csvStreamOutput = CSVHelper.CSVStringToStreamFile(inputCSVString);

            // assert
            var textWithinCSV = Encoding.UTF8.GetString(csvStreamOutput.ToArray());
            textWithinCSV.Should().Be(inputCSVString);
        }

        [Fact]
        public void CSVHelperCreatesCSVStreamBasedOnInputModelCollection()
        {
            // arrange
            var smallEasyToTestWithModels = RandomGen.CreateMany<GetPRNTransactionsDomain>(quantity: 3);

            var firstItem = smallEasyToTestWithModels[0];
            var secondItem = smallEasyToTestWithModels[1];
            var thirdItem = smallEasyToTestWithModels[2];

            firstItem.RentGroup = string.Empty;
            thirdItem.RentGroup = null;
            thirdItem.StartWeekOrMonth = default;

            var expectedCSVString = "RentGroup,FinancialYear,StartWeekOrMonth,EndWeekOrMonth"
                + $"\n{firstItem.RentGroup},{firstItem.FinancialYear},{firstItem.StartWeekOrMonth},{firstItem.EndWeekOrMonth}"
                + $"\n{secondItem.RentGroup},{secondItem.FinancialYear},{secondItem.StartWeekOrMonth},{secondItem.EndWeekOrMonth}"
                + $"\n{thirdItem.RentGroup},{thirdItem.FinancialYear},{thirdItem.StartWeekOrMonth},{thirdItem.EndWeekOrMonth}";

            // act
            var csvStreamOutput = CSVHelper.ToCSVStreamFile(smallEasyToTestWithModels);

            // assert
            var textWithinCSV = Encoding.UTF8.GetString(csvStreamOutput.ToArray());
            textWithinCSV.Should().Be(expectedCSVString);
        }

        [Fact]
        public void CSVHelperCanHandleNullItemsWithinCollections()
        {
            // arrange
            var modelCollectionWithNulls = new List<GetPRNTransactionsDomain>() { null };

            var expectedCSVString = "RentGroup,FinancialYear,StartWeekOrMonth,EndWeekOrMonth";

            // act
            var csvStringOutput = CSVHelper.ToCSVString(modelCollectionWithNulls);

            // assert
            csvStringOutput.Should().Be(expectedCSVString);
        }

        [Fact]
        public void CSVHelperCanHandleNullCollections()
        {
            // arrange
            var nullModelCollection = null as List<GetPRNTransactionsDomain>;

            var expectedCSVString = "RentGroup,FinancialYear,StartWeekOrMonth,EndWeekOrMonth";

            // act
            var csvStringOutput = CSVHelper.ToCSVString(nullModelCollection);

            // assert
            csvStringOutput.Should().Be(expectedCSVString);
        }

        [Fact]
        public void CSVHelperCanHandleFieldlessModels()
        {
            // arrange
            var itemCount = 3;

            var listOfFieldlessModels = Enumerable
                .Range(0, itemCount)
                .Select(i => new { })
                .ToList();

            var expectedCSVRows = Enumerable.Repeat(
                element: string.Empty,
                count: itemCount + 1 // collection items + header
            );

            var expectedCSVString = string.Join(separator: '\n', expectedCSVRows);

            // act
            var csvStringOutput = CSVHelper.ToCSVString(listOfFieldlessModels);

            // assert
            csvStringOutput.Should().Be(expectedCSVString);
        }
    }
}
