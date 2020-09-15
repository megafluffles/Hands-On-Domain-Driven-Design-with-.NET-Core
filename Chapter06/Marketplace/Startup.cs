using System.Threading.Tasks;
using Marketplace.Api;
using Marketplace.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Raven.Client.Documents;

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
            var store = new DocumentStore
            {
                Urls = new[] {"http://localhost:8080"},
                Database = "Marketplace_Chapter6",
                Conventions =
                {
                    FindIdentityProperty = m => m.Name == "_databaseId"
                }
            };
            store.Conventions.RegisterAsyncIdConvention<ClassifiedAd>(
                (dbName, entity) => Task.FromResult("ClassifiedAd/" + entity.Id));
            store.Initialize();

            services.AddTransient(c => store.OpenAsyncSession());
            services.AddSingleton<ICurrencyLookup, FixedCurrencyLookup>();
            services.AddScoped<IClassifiedAdRepository, ClassifiedAdRepository>();
            services.AddSingleton<ClassifiedAdsApplicationService>();

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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClassifiedAds v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}