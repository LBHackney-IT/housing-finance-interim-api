using HousingFinanceInterimApi.V1.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    [TestFixture]
    public class ThrowOpsErrorUsecaseTests
    {
        [Test]
        public void ThrowsTestOpsErrorException()
        {
            TestOpsErrorException ex = Assert.Throws<TestOpsErrorException>(
                delegate { ThrowOpsErrorUsecase.Execute(); });

            string expected = "This is a test exception to test our integrations";

            ex.Message.Should().BeEquivalentTo(expected);
        }
    }
}
