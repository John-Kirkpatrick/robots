using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Robots.API.Application.Options;
using Robots.Core.Behaviors;
using Robots.Core.Clients;
using Robots.Core.Commands;
using Robots.Core.Queries;
using Robots.Core.Services;
using Robots.Data.Contexts;
using Robots.Domain.Models;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;

namespace Robots.API
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
      // https://docs.microsoft.com/en-us/ef/core/performance/advanced-performance-topics?tabs=with-di%2Cwith-constant#dbcontext-pooling
      services.AddDbContextPool<RobotContext>(options =>
                                                {
                                                  options.UseNpgsql(configuration.GetConnectionString("Postgres"));
                                                });
      return services;
    }

    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
      services.AddScoped<IRobotQueries, RobotQueries>()
              .AddScoped<IJobQueries, JobQueries>();
      return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
      services.AddScoped<IRobotService, RobotService>();
      return services;
    }

    public static IServiceCollection ConfigureMediator(this IServiceCollection services)
    {
      services.AddMediatR(Assembly.GetAssembly(typeof(StoreJobCommand))!);
      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
      return services;
    }

    public static void AddDistributedCache(this IServiceCollection services, DistributedCacheOptions? cacheOptions)
    {
      services.AddOptions();
      services.AddStackExchangeRedisCache(options =>
                                          {
                                            options.Configuration = cacheOptions?.RedisConfiguration;
                                            options.InstanceName = cacheOptions?.RedisInstanceName;
                                          });
    }

    public static void ConfigureLogging(this ILoggingBuilder builder, HostBuilderContext context)
    {
      builder.ClearProviders();
      var serilogConfig = new LoggerConfiguration().ReadFrom.Configuration(context.Configuration)
                                                   .Enrich.FromLogContext()
                                                   .Enrich.WithThreadId()
                                                   .Enrich.WithMachineName()
                                                   .Enrich.WithExceptionDetails()
                                                   .Enrich.WithThreadId()
                                                   .Enrich.WithProperty("EnvironmentName", context.HostingEnvironment.EnvironmentName)
                                                   .Enrich.WithMachineName()
                                                   .WriteTo.Console(new JsonFormatter(renderMessage: true));

      Log.Logger = serilogConfig.CreateLogger();
      builder.AddSerilog(Log.Logger);
    }

    public static IServiceCollection AddClients(this IServiceCollection services)
    {
      services.AddHttpClient<IRobotsClient, RobotsClient>(ConfigureRobotsClient)
              .AddPolicyHandler(RetryPolicy);

      return services;
    }

    public static IServiceCollection ConfigureMapping(this IServiceCollection services)
    {
      var config = new MapperConfiguration(_ => { });

      config.AssertConfigurationIsValid();

      IMapper mapper = config.CreateMapper();
      services.AddSingleton(mapper);
      services.AddAutoMapper(typeof(RobotContract).GetTypeInfo().Assembly);

      return services;
    }

    private static readonly Action<IServiceProvider, HttpClient> ConfigureRobotsClient = (provider, client) =>
                                                                                            {
                                                                                              var options = provider.GetService<IOptions<RobotsClientOptions>>()!;
                                                                                              client.BaseAddress = new Uri(options.Value.BaseUri);
                                                                                              client.Timeout = TimeSpan.FromSeconds(30);
                                                                                            };

    private static IAsyncPolicy<HttpResponseMessage> RetryPolicy => HttpPolicyExtensions.HandleTransientHttpError()
                                                                                        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                                                                                        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
  }
}