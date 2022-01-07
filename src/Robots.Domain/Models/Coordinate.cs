namespace Robots.Domain.Models
{
  public struct Coordinate
  {
    public Coordinate(int x, int y)
    {
      X = x;
      Y = y;
    }

    public int X { get; init; }
    public int Y { get; init; }

    public override string ToString() => $"({X}, {Y})";
  }
}
