using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Robots.Data.Contexts;
using Robots.Domain.Entities;

namespace Utility
{
  public class ContextUtility
  {
    public static RobotContext GetRobotMemoryContext()
    {
      var context = new RobotContext(MemoryContextOptions<RobotContext>());

      var jobs = FakeIt.FakeList<Job>(10);
      context.Jobs.AddRange(jobs);
      context.SaveChanges();
      return context;
    }

    private static DbContextOptions<T> MemoryContextOptions<T>() where T : DbContext
    {
      // Create a new service provider to create a new in-memory database.
      ServiceProvider serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

      // Create a new options instance using an in-memory database and 
      // IServiceProvider that the context should resolve all of its 
      // services from.
      DbContextOptionsBuilder<T> builder = new DbContextOptionsBuilder<T>()
                                           .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                           .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                                           .UseInternalServiceProvider(serviceProvider)
                                           .EnableDetailedErrors()
                                           .EnableSensitiveDataLogging();

      return builder.Options;
    }
  }
}
