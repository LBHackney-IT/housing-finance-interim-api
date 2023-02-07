using System;

namespace HousingFinanceInterimApi.V1.Exceptions
{
    public class NoFileSettingsFoundException : Exception
    {
        public NoFileSettingsFoundException(string label)
            : base($"No file settings with label: '{label}' were found.") { }
    }
}
