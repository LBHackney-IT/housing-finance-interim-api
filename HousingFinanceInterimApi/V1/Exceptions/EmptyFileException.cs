using System;

namespace HousingFinanceInterimApi.V1.Exceptions
{
    public class EmptyFileException : Exception
    {
        public EmptyFileException(string fileName) : base($"No rows found in file {fileName}") { }
    }
}
