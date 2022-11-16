using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.Versioning;
using Hackney.Core.DynamoDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;

namespace HousingFinanceInterimApi
{

    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        private static List<ApiVersionDescription> _apiVersions { get; set; }

        private const string ApiName = "housing-finance-interim";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Setup configuration
            IConfigurationSection settingsSection = Configuration.GetSection("Settings");
            IConfigurationSection apiOptionsConfigSection = settingsSection.GetSection(nameof(ApiOptions));
            services.Configure<ApiOptions>(apiOptionsConfigSection);
            ApiOptions apiOptions = apiOptionsConfigSection.Get<ApiOptions>();


            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);

                // Assume that the caller wants the default version if they don't specify
                o.AssumeDefaultVersionWhenUnspecified = true;

                // Read the version number from the url segment header)
                o.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddSingleton<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Your Hackney Token. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });

                //Looks at the APIVersionAttribute [ApiVersion("x")] on controllers and decides whether or not
                //to include it in that version of the swagger document
                //Controllers must have this [ApiVersion("x")] to be included in swagger documentation!!
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    apiDesc.TryGetMethodInfo(out var methodInfo);

                    var versions = methodInfo?
                        .DeclaringType?.GetCustomAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions).ToList();

                    return versions?.Any(v => $"{v.GetFormattedApiVersion()}" == docName) ?? false;
                });

                //Get every ApiVersion attribute specified and create swagger docs for them
                foreach (var apiVersion in _apiVersions)
                {
                    var version = $"v{apiVersion.ApiVersion}";
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Title = $"{ApiName}-api {version}",
                        Version = version,
                        Description = $"{ApiName} version {version}. Please check older versions for depreciated endpoints."
                    });
                }

                c.CustomSchemaIds(x => x.FullName);
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            ConfigureDbContext(services);
            //services.ConfigureDynamoDB();

            RegisterGateways(services);
            RegisterUseCases(services);

            services.AddScoped<IGoogleClientServiceFactory, GoogleClientServiceFactory>();
        }

        private static void ConfigureDbContext(IServiceCollection services)
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            services.AddDbContext<DatabaseContext>(opt => opt.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(360);
            }));
        }

        private static void RegisterGateways(IServiceCollection services)
        {
            services.AddScoped<IOperatingBalanceGateway, OperatingBalanceGateway>();
            services.AddScoped<IPaymentGateway, PaymentGateway>();
            services.AddScoped<ITenancyGateway, TenancyGateway>();
            services.AddScoped<ITransactionGateway, TransactionGateway>();
            services.AddScoped<IBatchLogGateway, BatchLogGateway>();
            services.AddScoped<IBatchLogErrorGateway, BatchLogErrorGateway>();
            services.AddScoped<IReportGateway, ReportGateway>();
            services.AddScoped<IBatchReportGateway, BatchReportGateway>();
        }

        private static void RegisterUseCases(IServiceCollection services)
        {
            services.AddScoped<IGetBatchLogErrorUseCase, GetBatchLogErrorUseCase>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            var origins = Environment.GetEnvironmentVariable("ACCEPTED_ORIGINS").Split(",");
            app.UseCors(options => options.WithOrigins(origins)
                .AllowAnyMethod()
                .AllowAnyHeader());

            // Get All ApiVersions,
            IApiVersionDescriptionProvider api = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            _apiVersions = api.ApiVersionDescriptions.ToList();

            //Swagger ui to view the swagger.json file
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersionDescription in _apiVersions)
                {
                    //Create a swagger endpoint for each swagger version
                    c.SwaggerEndpoint($"{apiVersionDescription.GetFormattedApiVersion()}/swagger.json",
                        $"{ApiName}-api {apiVersionDescription.GetFormattedApiVersion()}");
                }
            });
            app.UseSwagger();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // SwaggerGen won't find controllers that are routed via this technique.
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }

}
