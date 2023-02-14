using System;
using System.Collections.Generic;

namespace HousingFinanceInterimApi.V1.Exceptions
{
    public class IncorrectFileNameException : Exception
    {
        public IncorrectFileNameException(string fileItemId, IList<string> fileItemParents) :
            base($"Non-standard cash filename (CashFileYYYYMMDD). Check file id: {fileItemId} in folder(s) {fileItemParents}")
        { }
    }
}