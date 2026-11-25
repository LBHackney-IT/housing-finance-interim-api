namespace HousingFinanceInterimApi.V1.Domain
{
    /// <summary>
    /// Implemented by spreadsheet domain models that have required fields.
    /// <see cref="GoogleClientService.ReadSheetToEntitiesAsync{_TEntity}"/> calls
    /// <see cref="IsValid"/> after deserialising each row and skips any row that
    /// returns false, logging the reason instead of crashing the import.
    /// </summary>
    public interface ISpreadsheetImportRow
    {
        bool IsValid(out string reason);
    }
}
