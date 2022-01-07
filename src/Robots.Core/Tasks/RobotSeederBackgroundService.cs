using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Robots.Core.Clients;
using Robots.Domain.Constants;

namespace Robots.Core.Tasks
{
  public class RobotSeederBackgroundService : BackgroundService
  {
    private readonly ILogger<RobotSeederBackgroundService> _logger;
    private readonly IServiceProvider _services;
    private readonly IDistributedCache _cache;
    private const int IntervalInSeconds = 5;

    public RobotSeederBackgroundService(ILogger<RobotSeederBackgroundService> logger,
                                        IServiceProvider services,
                                        IDistributedCache cache)
    {
      _logger = logger;
      _services = services;
      _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation($"{nameof(RobotSeederBackgroundService)} is starting");

      while (!stoppingToken.IsCancellationRequested)
      {
        _logger.LogDebug($"{nameof(RobotSeederBackgroundService)} is executing");
        await DoWorkAsync(stoppingToken);

        _logger.LogDebug($"{nameof(RobotSeederBackgroundService)} is going to sleep");
        await Task.Delay(TimeSpan.FromSeconds(IntervalInSeconds), stoppingToken);
      }
    }

    private async Task DoWorkAsync(CancellationToken stoppingToken)
    {
      try
      {
        using IServiceScope scope = _services.CreateScope();
        var robotsClient = scope.ServiceProvider.GetRequiredService<IRobotsClient>();
        var robots = await robotsClient.GetRobotsAsync(stoppingToken);

        _logger.LogInformation($"{nameof(RobotSeederBackgroundService)} is storing {robots.Length} robots to distributed cache");
        string serializedObjectToCache = JsonSerializer.Serialize(robots);
        await _cache.SetStringAsync(CacheKeys.RobotsCacheKey,
                                    serializedObjectToCache,
                                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(IntervalInSeconds * 100) },
                                    stoppingToken);
      }
      catch (Exception e)
      {
        _logger.LogError(e, e.GetBaseException().Message);
      }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
      _logger.LogWarning($"{nameof(RobotSeederBackgroundService)} is stopping");
      await base.StopAsync(stoppingToken);
    }
  }
}
