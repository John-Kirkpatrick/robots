using System.Linq;
using FluentAssertions;
using Utility;
using Xunit;

namespace UnitTests.Contexts
{
  public class RobotContextTests : IClassFixture<RobotContextFixture>
  {
    private readonly RobotContextFixture _fixture;

    public RobotContextTests(RobotContextFixture fixture)
    {
      _fixture = fixture;
    }

    [Fact]
    public void Can_Seed_InMemory_Context()
    {
      _fixture.Context.Jobs.Any()
              .Should()
              .BeTrue();
    }
  }
}
