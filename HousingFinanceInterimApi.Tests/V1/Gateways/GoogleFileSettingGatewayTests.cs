using System;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
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
                    Id = i,
                    GoogleFolderId = i.ToString(),
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
        /// <param name="value">The value.</param>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task CanGet(int value)
        {
            var listResult = await _gateway.ListAsync().ConfigureAwait(false);

            Assert.NotNull(listResult);
            Assert.NotEmpty(listResult);
            Assert.Contains(value, listResult.Select(item => item.Id));
        }

    }

}
