namespace Robots.API.Application.Options
{
  public class DistributedCacheOptions
  {
    public string RedisConfiguration { get; set; } = null!;

    public string RedisInstanceName { get; set; } = null!;
  }
}
