using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.Versioning;
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
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using HousingFinanceInterimApi.V1.Domain.AutoMaps;

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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Setup configuration
            IConfigurationSection settingsSection = Configuration.GetSection("Settings");
            IConfigurationSection apiOptionsConfigSection = settingsSection.GetSection(nameof(ApiOptions));
            services.Configure<ApiOptions>(apiOptionsConfigSection);
            ApiOptions apiOptions = apiOptionsConfigSection.Get<ApiOptions>();

            // Add auto mapper
            var mapperConfig = new MapperConfiguration(mapperConfiguration =>
            {
                mapperConfiguration.AddProfile(new RentBreakdownMappingProfile());
                mapperConfiguration.AddProfile(new CurrentRentPositionMappingProfile());
                mapperConfiguration.AddProfile(new LeaseholdAccountMappingProfile());
                mapperConfiguration.AddProfile(new ServiceChargePaymentsReceivedMappingProfile());
                mapperConfiguration.AddProfile(new GarageMappingProfile());
                mapperConfiguration.AddProfile(new OtherHRAMappingProfile());
            });
            services.AddSingleton(mapperConfig.CreateMapper());

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

            services.AddSwaggerGen(swaggerSetup =>
            {
                //swaggerSetup.AddSecurityDefinition("Token", new OpenApiSecurityScheme
                //{
                //    In = ParameterLocation.Header,
                //    // TODO ensure populated
                //    Description = apiOptions.HackneyApiKey,
                //    Name = "X-Api-Key",
                //    Type = SecuritySchemeType.ApiKey
                //});

                swaggerSetup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme, Id = "Token"
                            }
                        },
                        new List<string>()
                    }
                });

                // Looks at the APIVersionAttribute [ApiVersion("x")] on controllers and decides whether or not
                // to include it in that version of the swagger document
                // Controllers must have this [ApiVersion("x")] to be included in swagger documentation!!
                swaggerSetup.DocInclusionPredicate((docName, apiDesc) =>
                {
                    apiDesc.TryGetMethodInfo(out MethodInfo methodInfo);

                    IList<ApiVersion> versions = methodInfo?.DeclaringType?.GetCustomAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions)
                        .ToList();

                    return versions?.Any(v => $"{v.GetFormattedApiVersion()}" == docName) ?? false;
                });

                // Get every ApiVersion attribute specified and create swagger docs for them
                foreach (string version in _apiVersions.Select(apiVersion => $"v{apiVersion.ApiVersion}"))
                {
                    swaggerSetup.SwaggerDoc(version, new OpenApiInfo
                    {
                        Title = $"{apiOptions.ApiName}-api {version}",
                        Version = version,
                        Description =
                            $"{apiOptions.ApiName} version {version}. Please check older versions for depreciated endpoints."
                    });
                }

                swaggerSetup.CustomSchemaIds(x => x.FullName);

                // Set the comments path for the Swagger JSON and UI.
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    swaggerSetup.IncludeXmlComments(xmlPath);
                }
            });
            ConfigureDbContext(services);
            RegisterGateways(services);
            RegisterUseCases(services);

            services.AddScoped<IGoogleClientServiceFactory, GoogleClientServiceFactory>();
        }

        private static void ConfigureDbContext(IServiceCollection services)
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            services.AddDbContext<DatabaseContext>(opt => opt.UseSqlServer(connectionString));
        }

        private static void RegisterGateways(IServiceCollection services)
        {
            services.AddScoped<IOperatingBalanceGateway, OperatingBalanceGateway>();
            services.AddScoped<IPaymentGateway, PaymentGateway>();
            services.AddScoped<ITenancyGateway, TenancyGateway>();
        }

        private static void RegisterUseCases(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<ApiOptions> apiOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(options => options.WithOrigins("http://localhost:44335", "https://dmg8fqy2zxv7c.cloudfront.net")
                .AllowAnyMethod()
                .AllowAnyHeader());

            // Get All ApiVersions,
            IApiVersionDescriptionProvider api = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            _apiVersions = api.ApiVersionDescriptions.ToList();

            // Swagger ui to view the swagger.json file
            app.UseSwaggerUI(c =>
            {
                foreach (ApiVersionDescription apiVersionDescription in _apiVersions)
                {
                    //Create a swagger endpoint for each swagger version
                    c.SwaggerEndpoint($"{apiVersionDescription.GetFormattedApiVersion()}/swagger.json",
                        $"{apiOptions.Value.ApiName}-api {apiVersionDescription.GetFormattedApiVersion()}");
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
