using System.Collections.Generic;
using System.IO;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Helpers
{
    public static class CSVHelper
    {
        public static string ToCSVString<T>(IList<T> modelCollection) where T : class, new()
        {
            var properties = typeof(T).GetProperties();

            if (modelCollection is null)
                modelCollection = new List<T>();

            var csvModel = modelCollection
                .Where(item => item is not null)
                .Select(
                    item => properties
                        .Select(p => p.GetValue(item)?.ToString())
                        .ToList()
                )
                .ToList();

            var headers = properties.Select(p => p.Name).ToList();

            csvModel.Insert(0, headers);

            var csvRows = csvModel.Select(row => string.Join(',', row));
            var csvString = string.Join('\n', csvRows);

            return csvString;
        }

        public static MemoryStream CSVStringToStreamFile(string csvString)
        {
            var csvFile = new MemoryStream();

            using (TextWriter tw = new StreamWriter(stream: csvFile, leaveOpen: true))
            {
                tw.Write(csvString);
                tw.Flush();
            }

            return csvFile;
        }

        public static MemoryStream ToCSVStreamFile<T>(IList<T> modelCollection) where T : class, new()
        {
            var csvString = ToCSVString(modelCollection);
            return CSVStringToStreamFile(csvString);
        }

        public static FileInMemory ToCSVInMemoryFile<T>(IList<T> modelCollection, string fileName) where T : class, new()
        {
            var csvStream = ToCSVStreamFile(modelCollection);

            return new FileInMemory(
                DataStream: csvStream,
                Name: fileName,
                MimeType: "text/csv"
            );
        }
    }
}
