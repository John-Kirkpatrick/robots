using System.IO;
using System.Threading;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Robots.API;
using Robots.Core.Commands;
using Robots.Core.Queries;
using Robots.Domain.Models;
using Utility;

namespace FunctionalTests
{
  public class RobotsWebApplicationFactory : WebApplicationFactory<Startup>
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      var fakeRobots = FakeIt.FakeList<RobotContract>(10);
      var mockRobotQueries = new Mock<IRobotQueries>();
      mockRobotQueries.Setup(x => x.GetRobotsAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(fakeRobots.ToArray());

      var mockMediator = new Mock<IMediator>();
      mockMediator.Setup(x => x.Send(It.IsAny<StoreJobCommand>(), It.IsAny<CancellationToken>())).Verifiable();

      var memoryContext = ContextUtility.GetRobotMemoryContext();
      builder.UseContentRoot(Path.GetDirectoryName(Directory.GetCurrentDirectory())!)
             .UseEnvironment("Test")
             .UseTestServer()
             .ConfigureTestServices(services =>
             {
               services.AddSingleton(_ => mockRobotQueries.Object);
               services.AddSingleton(_ => mockMediator.Object);
               services.AddSingleton(_ => memoryContext);
             });
    }

    protected override IHostBuilder CreateHostBuilder()
    {
      return Host.CreateDefaultBuilder()
                 .ConfigureAppConfiguration((_, builder) =>
                                            {
                                              builder.AddEnvironmentVariables();
                                            })
                 .ConfigureWebHostDefaults(builder =>
                                           {
                                             builder.UseContentRoot(Directory.GetCurrentDirectory())
                                                    .UseStartup<Startup>();
                                           })
                 .UseDefaultServiceProvider((_, options) =>
                                            {
                                              options.ValidateOnBuild = true;
                                            });
    }
  }
}