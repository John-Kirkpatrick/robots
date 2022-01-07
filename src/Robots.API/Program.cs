using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Robots.Core.Extensions;
using Robots.Data.Contexts;
using Serilog;

namespace Robots.API
{
  public class Program
  {
    public static int Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration().MinimumLevel.Information().Enrich.FromLogContext().Destructure.ToMaximumDepth(100).CreateLogger();

      try
      {
        Log.Information("Configuring web host..");
        var host = CreateHostBuilder(args).Build();

        Log.Information("Applying migrations...");
        host.MigrateDbContext<RobotContext>((context, services) =>
        {
          // TODO: Add data seeder
        });

        Log.Information("Starting host..");
        host.Run();

        return 0;
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Program terminated unexpectedly");
        return 1;
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureAppConfiguration((_, builder) =>
          {
            builder.AddKeyPerFile("/run/secrets", true).AddEnvironmentVariables().AddUserSecrets<Startup>(true);
          })
          .ConfigureLogging((context, builder) =>
          {
            builder.ConfigureLogging(context);
          })
          .ConfigureWebHostDefaults(builder =>
          {
            builder.UseContentRoot(Directory.GetCurrentDirectory())
                   .CaptureStartupErrors(true)
                   .UseStartup<Startup>()
                   .UseShutdownTimeout(TimeSpan.FromSeconds(10));
          })
          .UseSerilog();
  }
}