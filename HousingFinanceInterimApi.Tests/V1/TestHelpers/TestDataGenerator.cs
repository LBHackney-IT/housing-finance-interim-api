using System;
using AutoFixture;

namespace HousingFinanceInterimApi.Tests.V1.TestHelpers
{
    public static class TestDataGenerator
    {
        private static readonly Fixture _fixture = new();

        /// <summary>
        /// Property Reference known locally as  prop_ref
        /// </summary>
        /// <returns></returns>
        public static string PopRef
        {
            get
            {
                return string.Concat("9", _fixture.Create<int>().ToString().PadLeft(7, '0').AsSpan(0, 7));
            }
        }
    }
}
