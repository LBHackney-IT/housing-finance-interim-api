using System;
using AutoFixture;

namespace HousingFinanceInterimApi.Tests.V1.TestHelpers
{
    public static class TestDataGenerator
    {
        private static readonly Fixture _fixture = new();
        private static readonly Bogus.Faker _faker = new();

        /// <summary>
        /// Property Reference known locally as  prop_ref
        /// </summary>
        /// <returns></returns>
        public static string PropRef
        {
            get
            {
                return string.Concat("9", _fixture.Create<int>().ToString().PadLeft(7, '0').AsSpan(0, 7));
            }
        }

        public static string RentAccount()
        {
            return _faker.Random.Long(0, 9_999_999_999).ToString().PadLeft(10, '0');
        }
    }
}
