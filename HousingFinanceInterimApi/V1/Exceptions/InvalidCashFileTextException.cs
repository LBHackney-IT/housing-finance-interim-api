using System;

namespace HousingFinanceInterimApi.V1.Exceptions
{
    public class InvalidCashFileTextException : Exception
    {
        public InvalidCashFileTextException(string message) : base(message) { }
    }
}
