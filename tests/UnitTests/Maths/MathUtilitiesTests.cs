using FluentAssertions;
using Robots.Core;
using Robots.Domain.Models;
using Xunit;

namespace UnitTests.Maths
{
  public class MathUtilitiesTests
  {
    [Fact]
    public void Can_Compute_Radicand()
    {
      // arrange
      var pointA = new Coordinate(25, 10);
      var pointB = new Coordinate(50, 55);
      var expected = 2650;

      // act
      var radicand = MathUtilities.EuclideanRadicand(pointA, pointB);

      // assert
      radicand.Should().Be(expected);
    }

    [Fact]
    public void Can_Compute_Robots_Radicands()
    {
      // arrange
      var robot = new RobotContract() { RobotId = 1, BatteryLevel = 100, X = 25, Y = 10 };
      var destination = new Coordinate(50, 55);
      var expected = 2650;

      // act
      var results = MathUtilities.ComputeRobotsRadicands(new RobotContract[] { robot }, destination);

      // assert
      results[0].Radicand.Should().Be(expected);
    }
  }
}
