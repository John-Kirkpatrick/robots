using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Robots.Core.Commands;
using Robots.Core.Queries;
using Robots.Domain.Models;

namespace Robots.Core.Services
{
  public interface IRobotService
  {
    /// <summary>
    /// Returns which robot is the best to transport the load based on which one is closest the load's location
    /// Requirements:
    /// (1): If there is more than 1 robot within 10 distance units of the load, return the one with the most battery remaining
    /// </summary>
    /// <param name="load"></param>
    /// <param name="token"></param>
    /// <returns>NearestRobotResult</returns>
    Task<NearestRobotResult> GetNearestRobotAsync(PayLoad load, CancellationToken token);
  }

  public class RobotService : IRobotService
  {
    private readonly IRobotQueries _robotQueries;
    private readonly ILogger<RobotService> _logger;
    private readonly IMediator _mediator;

    public RobotService(IMediator mediator, IRobotQueries robotQueries, ILogger<RobotService> logger)
    {
      _mediator = mediator;
      _robotQueries = robotQueries;
      _logger = logger;
    }

    /// <inheritdoc />
    public async Task<NearestRobotResult> GetNearestRobotAsync(PayLoad load, CancellationToken token)
    {
      _logger.LogDebug($"Processing payload {load.LoadId}");
      var robots = (await _robotQueries.GetRobotsAsync(token))
        .Where(x => x.BatteryLevel > 3) // low power robots will be sent to recharge
        .ToArray();

      var radicands = MathUtilities.ComputeRobotsRadicands(robots, load.Location);
      var closeRobots = GetCloseRobots(radicands);

      // Requirement (1)
      var hasMultipleInRange = closeRobots.Count > 1;
      var nearest = hasMultipleInRange
                      ? closeRobots.OrderByDescending(x => x.BatteryLevel).First()
                      : closeRobots.First();

      await StoreResult(load, nearest, token);
      return nearest;
    }

    private static List<NearestRobotResult> GetCloseRobots(IReadOnlyCollection<(RobotContract Robot, double Radicand)> radicands)
    {
      // order the radicands nearest to farthest
      var orderedRadicands = radicands.OrderBy(t => t.Radicand).ToArray();
      var closeRobots = new List<NearestRobotResult>();

      for (int index = 0; index < radicands.Count; index++)
      {
        var item = orderedRadicands[index];

        // based on radicand compute the distance
        var distance = Math.Sqrt(item.Radicand);

        // if distance < 10 or the list is empty, add the item
        if (distance < 10 || closeRobots.Count == 0)
        {
          closeRobots.Add(new NearestRobotResult() {
            RobotId = item.Robot.RobotId,
            BatteryLevel = item.Robot.BatteryLevel,
            DistanceToGoal = distance
          });
        }
        else
        {
          break;
        }
      }

      return closeRobots;
    }

    private async Task StoreResult(PayLoad load, NearestRobotResult nearest, CancellationToken token)
    {
      var command = new StoreJobCommand(Guid.NewGuid(), load.LoadId, nearest.RobotId, nearest.DistanceToGoal, nearest.BatteryLevel);
      await _mediator.Send(command, token);
    }
  }
}
