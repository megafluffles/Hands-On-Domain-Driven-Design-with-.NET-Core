using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
using Marketplace.ClassifiedAd;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.Projections;
using Marketplace.UserProfile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// ReSharper disable UnusedMember.Global

namespace Marketplace
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var esConnection = EventStoreConnection.Create(
                Configuration["eventStore:connectionString"],
                ConnectionSettings.Create().DisableTls().KeepReconnecting(),
                Environment.ApplicationName);
            var store = new EsAggregateStore(esConnection);
            var purgomalumClient = new PurgomalumClient();

            services.AddSingleton(esConnection);
            services.AddSingleton<IAggregateStore>(store);

            services.AddSingleton(new ClassifiedAdsApplicationService(
                store, new FixedCurrencyLookup()));
            services.AddSingleton(new UserProfileApplicationService(
                store, t => purgomalumClient.CheckForProfanity(t)));

            var classifiedAdDetails = new List<ReadModels.ClassifiedAdDetails>();
            services.AddSingleton<IEnumerable<ReadModels.ClassifiedAdDetails>>(classifiedAdDetails);
            var userDetails = new List<ReadModels.UserDetails>();
            services.AddSingleton<IEnumerable<ReadModels.UserDetails>>(userDetails);

            var projectionManager = new ProjectionManager(esConnection,
                new ClassifiedAdDetailsProjection(classifiedAdDetails,
                    userId => userDetails.FirstOrDefault(x => x.UserId == userId)?.DisplayName),
                new UserDetailsProjection(userDetails),
                new ClassifiedAdUpcasters(esConnection,
                    userId => userDetails.FirstOrDefault(x => x.UserId == userId)?.PhotoUrl));

            services.AddSingleton<IHostedService>(
                new EventStoreService(esConnection, projectionManager));

            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "ClassifiedAds",
                        Version = "v1"
                    });
            });

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            };
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClassifiedAds v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}