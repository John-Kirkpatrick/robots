using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Robots.Domain.Constants;
using Robots.Domain.Models;

namespace Robots.Core.Queries
{
  public interface IRobotQueries
  {
    Task<RobotContract[]> GetRobotsAsync(CancellationToken token);
  }

  public class RobotQueries : IRobotQueries
  {
    private readonly IDistributedCache _cache;
    private readonly ILogger<RobotQueries> _logger;

    public RobotQueries(IDistributedCache cache, ILogger<RobotQueries> logger)
    {
      _cache = cache;
      _logger = logger;
    }

    public async Task<RobotContract[]> GetRobotsAsync(CancellationToken token)
    {
      RobotContract[]? robots = null;

      try
      {
        _logger.LogDebug("Retrieving customers from cache");
        string? jsonObject = await _cache.GetStringAsync(CacheKeys.RobotsCacheKey, token);
        robots = JsonSerializer.Deserialize<RobotContract[]>(jsonObject);
      }

      catch (Exception exc)
      {
        _logger.LogError(new EventId(exc.HResult), exc, exc.GetBaseException().Message);
      }

      return robots ?? throw new KeyNotFoundException("Unable to retrieve robots");
    }
  }
}
