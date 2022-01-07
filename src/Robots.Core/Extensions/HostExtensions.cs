using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace Robots.Core.Extensions
{
  public static class HostExtensions
  {
    public static IHost MigrateDbContext<TContext>(this IHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
    {
      using var scope = webHost.Services.CreateScope();
      var services = scope.ServiceProvider;
      var logger = services.GetRequiredService<ILogger<TContext>>();
      var context = services.GetService<TContext>();

      try
      {
        logger.LogInformation("Migrating database");

        var retry = Policy.Handle<Exception>()
                          .WaitAndRetry(new[]
                                        {
                                          TimeSpan.FromSeconds(5),
                                          TimeSpan.FromSeconds(10),
                                          TimeSpan.FromSeconds(15)
                                        });

        retry.Execute(() => InvokeSeeder(seeder, context!, services));
        logger.LogInformation("Migrated database");
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "An error occurred while migrating the database");
      }

      return webHost;
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services) where TContext : DbContext
    {
      context.Database.Migrate();
      seeder(context, services);
    }
  }
}
