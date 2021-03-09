using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Gateways.Options;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HousingFinanceInterimApi
{

    public class Handler
    {

        private readonly IGoogleClientService _googleClientService;

        public Handler()
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);
            DatabaseContext context = new DatabaseContext(optionsBuilder.Options);

            var options = Options.Create(new GoogleClientServiceOptions
            {
                ApplicationName = "",
                Scopes = new List<string>
                {
                    // TODO
                    "", ""
                }
            });
            IGoogleClientServiceFactory serviceFactory = new GoogleClientServiceFactory(default, options, context);

            _googleClientService =
                serviceFactory.CreateGoogleClientServiceForApiKey(Environment.GetEnvironmentVariable("GOOGLE_API_KEY"));
        }

        public async Task ImportFiles()
        {

        }

    }

}
