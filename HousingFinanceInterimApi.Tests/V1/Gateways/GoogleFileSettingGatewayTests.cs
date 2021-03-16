using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Gateways
{

    /// <summary>
    /// The Google settings tests.
    /// </summary>
    /// <seealso cref="DatabaseTests" />
    public class GoogleFileSettingGatewayTests : DatabaseTests
    {

        /// <summary>
        /// The testing gateway
        /// </summary>
        private readonly IGoogleFileSettingGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleFileSettingGatewayTests"/> class.
        /// </summary>
        public GoogleFileSettingGatewayTests()
        {
            // Seed
            for (int i = 1; i <= 3; i++)
            {
                DatabaseContext.GoogleFileSettings.Add(new GoogleFileSetting
                {
                    GoogleIdentifier = i.ToString(),
                    EndDate = DateTimeOffset.UtcNow.AddDays(1),
                    StartDate = DateTimeOffset.UtcNow,
                    FileType = ".dat"
                });
            }

            DatabaseContext.SaveChanges();
            _gateway = new GoogleFileSettingGateway(DatabaseContext);
        }

        /// <summary>
        /// Determines whether this instance can get the specified value.
        /// </summary>
        // TODO re-add [Fact]
        public async Task GetTest()
        {
            var listResult = await _gateway.ListAsync().ConfigureAwait(false);

            Assert.NotNull(listResult);
            Assert.NotEmpty(listResult);
            Assert.True(listResult.Count >= 3);
        }

    }

}
