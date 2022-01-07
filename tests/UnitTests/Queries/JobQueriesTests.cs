using System.Linq;
using FluentAssertions;
using Robots.Core.Queries;
using Utility;
using Xunit;

namespace UnitTests.Queries
{
  public class JobQueriesTests : IClassFixture<RobotContextFixture>
  {
    private readonly RobotContextFixture _fixture;

    public JobQueriesTests(RobotContextFixture fixture)
    {
      _fixture = fixture;
    }

    [Fact]
    public async void Can_Get_Jobs()
    {
      // arrange
      var queries = new JobQueries(_fixture.Context);

      // act
      var results = await queries.GetJobsAsync(default);

      // assert
      results.Count()
             .Should()
             .Be(_fixture.Context.Jobs.Count());
    }
  }
}
