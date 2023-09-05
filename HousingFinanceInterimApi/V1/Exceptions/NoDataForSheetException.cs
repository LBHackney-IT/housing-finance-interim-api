using System;
namespace HousingFinanceInterimApi.V1.Exceptions;

public class NoDataForSheetException : Exception
{
    public NoDataForSheetException(string dataKind, string sheetName, string spreadsheetId)
        : base($"No {dataKind} data to import. Sheet: ({sheetName}) on spreadsheet: ({spreadsheetId})")
    {
    }
}
