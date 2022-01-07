using System;
using System.Collections.Generic;
using Robots.Domain.Models;

namespace Robots.Core
{
  public static class MathUtilities
  {
    /// <summary>
    /// Computes the radicand of the euclidean distance formula
    /// Note: Does not use Math.Sqrt() or Math.Pow() for speed
    /// </summary>
    public static readonly Func<Coordinate, Coordinate, double> EuclideanRadicand = (p1, p2) =>
    {
      var diffX = p1.X - p2.X;
      var diffY = p1.Y - p2.Y;
      return (diffX * diffX + diffY * diffY);
    };

    /// <summary>
    /// Given an array of robots and their location, computes the euclidean radicands for each robot to the destination
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="destination"></param>
    /// <returns>The distance from each robot to the destination</returns>
    public static (RobotContract Robot, double Radicand)[] ComputeRobotsRadicands(RobotContract[] sequence, Coordinate destination)
    {
      var results = new (RobotContract Robot, double Radicand)[sequence.Length];
      for (int index = 0; index < sequence.Length; index++)
      {
        var robot = sequence[index];
        var radicand = EuclideanRadicand(robot.Location, destination);
        results[index] = new(robot, radicand);
      }

      return results;
    }

    public static double StandardDeviation(IEnumerable<double> sequence)
    {
      (double sum, double sumOfSquares, int count) = ComputeSumAndSumOfSquares(sequence);

      double variance = sumOfSquares - sum * sum / count;
      return Math.Sqrt(variance / count);
    }

    private static (double, double, int) ComputeSumAndSumOfSquares(IEnumerable<double> sequence)
    {
      double sum = 0;
      double sumOfSquares = 0;
      int count = 0;

      foreach (double item in sequence)
      {
        count++;
        sum += item;
        sumOfSquares += item * item;
      }

      return (sum, sumOfSquares, count);
    }
  }
}
