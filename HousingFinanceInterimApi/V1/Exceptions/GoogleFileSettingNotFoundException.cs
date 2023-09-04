using System;
namespace HousingFinanceInterimApi.Tests.V1.UseCase.Exceptions;

public class GoogleFileSettingNotFoundException: Exception
{
    public GoogleFileSettingNotFoundException(string googleFileSettingLabel)
        : base($"No Google File Settings found with label: {googleFileSettingLabel}.")
    {
    }
}
