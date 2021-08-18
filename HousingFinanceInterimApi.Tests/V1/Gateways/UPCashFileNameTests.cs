using System;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Gateways
{

    public class UPCashFileNameTests : DatabaseTests
    {

        private readonly IUPCashDumpFileNameGateway _fileNameGateway;

        public UPCashFileNameTests()
        {
            // Seed
            for (int i = 1; i <= 3; i++)
            {
                DatabaseContext.UpCashDumpFileNames.Add(new UPCashDumpFileName
                {
                    IsSuccess = true,
                    FileName = $"testfile{i}.dat",
                    Timestamp = DateTimeOffset.UtcNow
                });
            }

            DatabaseContext.SaveChanges();
            _fileNameGateway = new UPCashDumpFileNameGateway(DatabaseContext);
        }

        /// <summary>
        /// Performs a get test.
        /// </summary>
        /// <param name="value">The value to get with.</param>
        //TODO re-add [Theory]
        //[InlineData(1)]
        //[InlineData(2)]
        //[InlineData(3)]
        public async Task GetTest(int value)
        {
            string fileName = $"testfile{value}.dat";
            var getResult = await _fileNameGateway.GetAsync(fileName).ConfigureAwait(false);

            Assert.NotNull(getResult);
            Assert.Equal(fileName, getResult.FileName);
        }

        public async Task GetNotExists()
        {

        }

    }

}
