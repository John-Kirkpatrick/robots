using System.Text.Json.Serialization;

namespace Robots.Domain.Models
{
  public struct PayLoad
  {
    public PayLoad(int loadId, int x, int y)
    {
      LoadId = loadId;
      X = x;
      Y = y;
    }

    public int LoadId { get; set; }

    public int X { get; init; }

    public int Y { get; init; }

    [JsonIgnore]
    public Coordinate Location => new Coordinate(X, Y);
  }
}
