using System;
namespace HousingFinanceInterimApi.V1.Exceptions;

public class GoogleFileSettingNotFoundException : Exception
{
    public GoogleFileSettingNotFoundException(string googleFileSettingLabel)
        : base($"No Google File Settings found with label: {googleFileSettingLabel}.")
    {
    }
}
