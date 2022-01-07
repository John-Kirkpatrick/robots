using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Robots.Core.Queries;
using Robots.Domain.Constants;
using Robots.Domain.Models;
using Utility;
using Xunit;

namespace UnitTests.Queries
{
  public class RobotQueriesTests
  {
    [Fact]
    public async Task Can_Get_Robots()
    {
      // arrange
      var opts = Options.Create(new MemoryDistributedCacheOptions());
      IDistributedCache memoryCache = new MemoryDistributedCache(opts);

      var expected = FakeIt.FakeList<RobotContract>().ToArray();
      string serialized = JsonSerializer.Serialize(expected);
      await memoryCache.SetStringAsync(CacheKeys.RobotsCacheKey, serialized);
      var queries = new RobotQueries(memoryCache, new Logger<RobotQueries>(new NullLoggerFactory()));

      // act
      var results = await queries.GetRobotsAsync(default);

      // assert
      results.Should().BeEquivalentTo(expected);
    }
  }
}
