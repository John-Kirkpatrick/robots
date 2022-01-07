using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Robots.API.Application.Filters;
using Robots.API.Application.Options;
using Robots.Core.Commands;
using Robots.Core.Tasks;

namespace Robots.API
{
  public class Startup
  {
    public Startup(IConfiguration configuration) => Configuration = configuration;

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers(options => { options.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
              .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<StoreJobCommand>());

      services.Configure<RobotsClientOptions>(Configuration.GetSection("RobotsClient"));

      services.AddSwaggerGen(c =>
                             {
                               c.SwaggerDoc("v1", new OpenApiInfo { Title = "Robots.API", Version = "v1" });
                             });

      services.AddDbContexts(Configuration)
              .AddMemoryCache();

      var cacheOptions = Configuration.GetSection("DistributedCache").Get<DistributedCacheOptions>();
      services.AddDistributedCache(cacheOptions);

      services.AddHealthChecks()
              .AddNpgSql(Configuration.GetConnectionString("Postgres"), name: "postgres-check", tags: new[] { "postgres" });
      services.AddClients();

      services.AddQueries()
              .AddServices()
              .ConfigureMediator()
              .ConfigureMapping();
      services.AddHostedService<RobotSeederBackgroundService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      var basePath = Configuration["ASPNETCORE_BASEPATH"] ?? "";
      ReverseProxy(app, basePath);

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Robots.API v1"));
      app.UseRouting();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
                       {
                         endpoints.MapControllers();

                         endpoints.MapHealthChecks("/hc", new HealthCheckOptions { Predicate = _ => true });
                         endpoints.MapHealthChecks("/liveness", new HealthCheckOptions { Predicate = r => r.Name.Contains("self") });
                         endpoints.MapHealthChecks("/db", new HealthCheckOptions { Predicate = r => r.Name.Contains("postgres") });
                       });
    }

    private static void ReverseProxy(IApplicationBuilder app, string basePath)
    {
      // Base Path
      app.UsePathBase(new PathString(basePath));

      // Default Files with base path
      app.UseDefaultFiles(basePath);

      // Forwarded Headers
      var forwardedHeadersOptions = new ForwardedHeadersOptions {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
      };

      // Clear loopback address
      forwardedHeadersOptions.KnownProxies.Clear();
      forwardedHeadersOptions.KnownNetworks.Clear();
      app.UseForwardedHeaders(forwardedHeadersOptions);
    }
  }
}