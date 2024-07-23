using System;
using AutoFixture;

namespace HousingFinanceInterimApi.Tests.V1.Helper
{
    public static class TestDataGenerator
    {
        /// <summary>
        /// Property Reference aka prop_ref
        /// </summary>
        /// <returns></returns>
        public static string PropRef()
        {
            var fixture = new Fixture();
            // 8 Digit random number that leads with 9 to prevent clashes e.g. "90023871"
            return string.Concat("9", fixture.Create<int>().ToString().PadLeft(7, '0').AsSpan(0, 7));
        }
    }
}
