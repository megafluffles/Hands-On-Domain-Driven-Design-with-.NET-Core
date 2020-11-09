using System;
using EventStore.ClientAPI;
using Marketplace.ClassifiedAd;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.Projections;
using Marketplace.UserProfile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using SisoDb.JsonNet;
using Swashbuckle.AspNetCore.Swagger;
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

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
                ConnectionSettings.Create().KeepReconnecting(),
                Environment.ApplicationName);
            var store = new EsAggregateStore(esConnection);
            var purgomalumClient = new PurgomalumClient();
            var documentStore = ConfigureRavenDb(Configuration.GetSection("ravenDb"));

            Func<IAsyncDocumentSession> getSession = () => documentStore.OpenAsyncSession();

            services.AddTransient(c => getSession());

            services.AddSingleton(esConnection);
            services.AddSingleton<IAggregateStore>(store);

            services.AddSingleton(new ClassifiedAdsApplicationService(
                store, new FixedCurrencyLookup()));
            services.AddSingleton(new UserProfileApplicationService(
                store, t => purgomalumClient.CheckForProfanity(t)));

            var projectionManager = new ProjectionManager(esConnection,
                new RavenDbCheckpointStore(getSession, "readmodels"),
                new ClassifiedAdDetailsProjection(getSession,
                    async userId => (await getSession.GetUserDetails(userId))?.DisplayName),
                new ClassifiedAdUpcasters(esConnection,
                    async userId => (await getSession.GetUserDetails(userId))?.PhotoUrl),
                new UserDetailsProjection(getSession));

            services.AddSingleton<IHostedService>(
                new EventStoreService(esConnection, projectionManager));

            services.AddControllersWithViews();
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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClassifiedAds v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static IDocumentStore ConfigureRavenDb(IConfiguration configuration)
        {
            var store = new DocumentStore
            {
                Urls = new[] {configuration["server"]},
                Database = configuration["database"]
            };
            store.Initialize();
            var record = store.Maintenance.Server.Send(
                new GetDatabaseRecordOperation(store.Database));
            if (record == null)
            {
                store.Maintenance.Server.Send(
                    new CreateDatabaseOperation(new DatabaseRecord(store.Database)));
            }

            return store;
        }
    }
}