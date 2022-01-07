using System;

namespace Robots.Domain.Entities
{
  public class Job
  {
    public Guid Id { get; set; }

    public int PayloadId { get; set; }

    public int RobotId { get; set; }

    public double DistanceToGoal { get; set; }

    public int BatteryLevel { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;
  }
}
