using System;
using AutoFixture;
using Bogus;

namespace HousingFinanceInterimApi.Tests.V1.TestHelpers;


public static class TestDataGenerator
{
    private static readonly Fixture _fixture = new();
    private static readonly Faker _faker = new(locale: "en_GB");

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


public static class CashDumpTestData
{
    private static readonly Faker _faker = new(locale: "en_GB");
    public static string PaymentSource() =>
        _faker.Random.Word().Replace(" ", "").PadRight(10)[..10].ToUpper();
    public static string AmountPaid() =>
        _faker.Random.Decimal(0, 1000).ToString().PadLeft(9, '0')[..9];
    public static string PaymentDate() =>
        _faker.Date.Past().ToString("dd/MM/yyyy");
    public static string TransactionType() =>
        _faker.Random.AlphaNumeric(3).ToUpper();
    public static string CivicaCode() =>
        _faker.Random.Number(1, 99).ToString().PadLeft(2, '0');
    public static string FullTextBuild(string rentAccount, string paymentSource, string amountPaid, string paymentDate, string transactionType, string civicaCode) =>
        $"{rentAccount}{paymentSource}".PadRight(30) + $"{transactionType}+{amountPaid}{paymentDate}{civicaCode}";
    public static string FullText() =>
        FullTextBuild(TestDataGenerator.RentAccount(), PaymentSource(), AmountPaid(), PaymentDate(), TransactionType(), CivicaCode());
}

