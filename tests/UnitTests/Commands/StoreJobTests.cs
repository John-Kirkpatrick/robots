using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Robots.Core.Commands;
using Utility;
using Xunit;

namespace UnitTests.Commands
{
  public class StoreJobTests : IClassFixture<RobotContextFixture>
  {
    private readonly RobotContextFixture _fixture;

    public StoreJobTests(RobotContextFixture fixture)
    {
      _fixture = fixture;
    }

    [Fact]
    public async Task Command_Does_Persist()
    {
      // arrange
      var expected = Guid.NewGuid();
      var command = new StoreJobCommand(expected, 99, 2, 3.3, 4);
      var handler = new StoreJobHandler(_fixture.Context);
      var initialCount = _fixture.Context.Jobs.Count();
      initialCount.Should().BeGreaterThan(0);

      //act
      var result = await handler.Handle(command, default);

      //assert
      result.Should()
            .Be(expected);

      var newCount = _fixture.Context.Jobs.Count();

      newCount.Should().Be(initialCount + 1);

      // is idempotent
      await handler.Handle(command, default);
      await handler.Handle(command, default);
      await handler.Handle(command, default);

      _fixture.Context.Jobs.Count()
              .Should()
              .Be(newCount);
    }
  }
}
