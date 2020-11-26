using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.EF.Filters;
using OrchardCore.Modules;

namespace Codesanook.ThailandAdministrativeDivisionTool
{
    public class Startup : StartupBase
    {
        // https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-3.1
        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            // Migration, can be removed in a future release.
            services.AddSingleton<IDataMigration, Migrations>();

            // Register service action filter
            services.AddScoped<TransactionActionServiceFilter>();

            services.Configure<MvcOptions>((options) =>
            {
                // https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-5.0#dependency-injection
                options.Filters.Add(typeof(TransactionActionServiceFilter));  // Add global action filter by type
            });
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "Codesanook.ThailandAdministrativeDivisionTool",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}
