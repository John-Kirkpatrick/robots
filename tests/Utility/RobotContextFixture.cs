using Robots.Data.Contexts;

namespace Utility
{
  public class RobotContextFixture
  {
    public RobotContextFixture()
    {
      Context = ContextUtility.GetRobotMemoryContext();
    }

    public RobotContext Context { get; init; }
  }
}
