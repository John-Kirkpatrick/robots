using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Robots.Core.Commands;
using Robots.Core.Queries;
using Robots.Core.Services;
using Robots.Domain.Models;
using Xunit;

namespace UnitTests.Services
{
  public class RobotServiceTests
  {
    private readonly Mock<IRobotQueries> _mockRobotQueries;
    private readonly Coordinate _destination;
    private readonly Mock<IMediator> _mockMediator;

    public RobotServiceTests()
    {
      _mockRobotQueries = new Mock<IRobotQueries>();
      _destination = new Coordinate(50, 55);
      _mockMediator = new Mock<IMediator>();
      _mockMediator.Setup(x => x.Send(It.IsAny<StoreJobCommand>(), It.IsAny<CancellationToken>())).Verifiable();
    }

    [Fact]
    public async Task Can_Find_Nearest_Robot()
    {
      // arrange
      var expected = new RobotContract() { RobotId = 1, BatteryLevel = 100, X = _destination.X - 1, Y = _destination.Y - 1 };
      var testRobots = new[] { expected, new RobotContract() { RobotId = 2, BatteryLevel = 100, X = _destination.X - 10, Y = _destination.Y - 10 } };
      _mockRobotQueries.Setup(x => x.GetRobotsAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(testRobots)
                       .Verifiable();

      var service = new RobotService(_mockMediator.Object, _mockRobotQueries.Object, new Logger<RobotService>(new NullLoggerFactory()));

      // act
      var result = await service.GetNearestRobotAsync(new PayLoad() { LoadId = 1, X = _destination.X, Y = _destination.Y }, default);

      // assert
      result.RobotId.Should().Be(expected.RobotId);
      _mockMediator.Verify(x => x.Send(It.IsAny<StoreJobCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Returns_Robot_With_Max_Battery_If_Distance_Less_Than_10()
    {
      // arrange
      var closer = new RobotContract() { RobotId = 1, BatteryLevel = 10, X = _destination.X - 1, Y = _destination.Y - 1 };
      var morePower = new RobotContract() { RobotId = 2, BatteryLevel = 100, X = _destination.X - 3, Y = _destination.Y - 3 };
      var testRobots = new[] { closer, morePower };
      _mockRobotQueries.Setup(x => x.GetRobotsAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(testRobots)
                       .Verifiable();

      var service = new RobotService(_mockMediator.Object, _mockRobotQueries.Object, new Logger<RobotService>(new NullLoggerFactory()));

      // act
      var result = await service.GetNearestRobotAsync(new PayLoad() { LoadId = 1, X = _destination.X, Y = _destination.Y }, default);

      // assert
      result.RobotId.Should().Be(morePower.RobotId);
      _mockMediator.Verify(x => x.Send(It.IsAny<StoreJobCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
  }
}
