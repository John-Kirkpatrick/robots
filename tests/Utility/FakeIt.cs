using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;

namespace Utility
{
  public static class FakeIt
  {
    public static T FakeSingle<T>() where T : class
    {
      var fake = Builder<T>.CreateNew()
                           .Build();

      return fake;
    }

    public static List<T> FakeList<T>(int size = 10) where T : class
    {
      var retList = Builder<T>.CreateListOfSize(size)
                              .All()
                              .Build()
                              .ToList();

      return retList;
    }
  }
}
