using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Robots.Core.Clients;
using Xunit;

namespace IntegrationTests.Clients
{
  public class RobotsClientTests
  {
    [Fact]
    public async Task Client_Can_Get_Robots()
    {
      // arrange
      var httpClient = new HttpClient() { BaseAddress = new Uri("https://60c8ed887dafc90017ffbd56.mockapi.io/") };
      var client = new RobotsClient(httpClient, new Logger<RobotsClient>(new NullLoggerFactory()));

      // act
      var results = await client.GetRobotsAsync(default);

      // assert
      results.Length
             .Should()
             .Be(100);

      results.All(robot => robot.X > 0)
             .Should()
             .BeTrue();
    }
  }
}
