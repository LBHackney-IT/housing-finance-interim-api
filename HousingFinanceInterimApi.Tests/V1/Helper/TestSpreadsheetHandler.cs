using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using CsvHelper;
using Newtonsoft.Json;

namespace CautionaryAlertsApi.Tests.V1.Helper
{
    public class TestSpreadsheetHandler
    {
        private Dictionary<string, List<string>> _columnValues;

        public TestSpreadsheetHandler(string fileName)
        {
            Initialize(fileName);
        }

        public HttpResponseMessage RequestHandler(HttpRequestMessage request)
        {
            var queryParams = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var majorDimension = queryParams["majorDimension"] ?? "ROWS";
            var response = String.Empty;

            if (request.RequestUri.LocalPath.Contains("/values/", StringComparison.OrdinalIgnoreCase))
            {
                var valueRange = CreateValueRangeResponse(request.RequestUri.LocalPath, majorDimension);
                response = JsonConvert.SerializeObject(valueRange);
            }
            else if (request.RequestUri.LocalPath.Contains("/values:batch", StringComparison.InvariantCultureIgnoreCase))
            {
                // batch request accepts multiple ranges and wrap results into a wrapper with spreadsheetID
                // separate value range is created for each requested range.
                var ranges = queryParams["ranges"].Split(",");
                var valueRanges = ranges.Select(range => CreateValueRangeResponse(range, majorDimension));

                var batchResponse = new
                {
                    spreadSheetId = "CURRENT LIST",
                    valueRanges = valueRanges
                };

                response = JsonConvert.SerializeObject(batchResponse);
            }

            return new HttpResponseMessage { Content = new StringContent(response) };
        }

        private object CreateValueRangeResponse(string request, string majorDimension)
        {
            //range  is passed in URL as [CURRENT LIST!AH1:AH1000&majorDimension=COLUMNS]
            var match = Regex.Match(
                request,
                @"\!(?<range>(?<column1>[A-Z]{1,2})(?<row1>\d{1,4}):(?<column2>[A-Z]{1,2})(?<row2>\d{1,4}))");

            var startColumn = match.Groups["column1"].Value;
            var endColumn = match.Groups["column2"].Value;

            var startIndex = Convert.ToInt32(match.Groups["row1"].Value);
            var endIndex = Convert.ToInt32(match.Groups["row2"].Value);

            /* value range response looks like this
            {
              "range": "CURRENT SHEET!A3:P3",
              "majorDimension": "ROW",
              "values": [
                  [ "row 1 column 1", "row 1 column 2" ],
                  [ "row 2 column 1", "row 2 column 2" ]
               ]
            }
            if majorDimension is set to ROWS each element in values array represents single row (see above)
            if majorDimension is set to COLUMNS each element in values array represents single column:
              "values": [
                  [ "row 1 column 1", "row 2 column 1" ],
                  [ "row 1 column 2", "row 2 column 2" ]
               ]
           */

            var values = majorDimension == "ROWS"
                ? ReadRowsRange(startColumn, endColumn, startIndex, endIndex)
                : ReadColumnsRange(startColumn, endColumn, startIndex, endIndex);

            return new
            {
                range = $"CURRENT SHEET!{match.Groups["range"]}",
                majorDimension = majorDimension,
                values = values.ToArray()
            };
        }

        private IEnumerable<string[]> ReadRowsRange(string startColumn, string endColumn, int startIndex, int endIndex)
        {
            var maxIndex = Math.Min(endIndex, _columnValues.First().Value.Count) - 1;

            for (var rowIndex = startIndex - 1; rowIndex <= maxIndex; rowIndex++)
            {
                var rowData = new List<string>();

                foreach (var column in _columnValues.Keys)
                {
                    if (IsColumnInRange(column, startColumn, endColumn))
                    {
                        rowData.Add(_columnValues[column][rowIndex]);
                    }
                }

                if (rowData.Count > 0) yield return rowData.ToArray();
            }
        }

        private IEnumerable<string[]> ReadColumnsRange(string startColumn, string endColumn, int startIndex, int endIndex)
        {
            foreach (var column in _columnValues.Keys)
            {
                if (IsColumnInRange(column, startColumn, endColumn))
                {
                    var values = _columnValues[column]
                        .Skip(startIndex - 1)
                        .Take(endIndex - startIndex + 1);

                    yield return values.ToArray();
                }
            }
        }

        private static bool IsColumnInRange(string column, string start, string end)
        {
            return start.Length <= column.Length &&
                   column.Length <= end.Length &&
                   String.Compare(start, column, StringComparison.OrdinalIgnoreCase) <= 0 &&
                   String.Compare(column, end, StringComparison.OrdinalIgnoreCase) <= 0;
        }

        private void Initialize(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceNames = assembly.GetManifestResourceNames();
            var resourceName = resourceNames.Single(str => str.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException(fileName);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();

            _columnValues = csv.HeaderRecord.ToDictionary(
                column => column,               // column header name
                column => new List<string>());  // list of column values

            while (csv.Read())
            {
                foreach (var column in csv.HeaderRecord)
                {
                    _columnValues[column].Add(csv.GetField(column));
                }
            }
        }
    }
}
