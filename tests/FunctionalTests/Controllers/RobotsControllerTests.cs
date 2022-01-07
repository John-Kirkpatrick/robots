using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Robots.Domain.Models;
using Xunit;

namespace FunctionalTests.Controllers
{
  public class RobotsControllerTests : IClassFixture<RobotsWebApplicationFactory>
  {
    private readonly RobotsWebApplicationFactory _factory;
    public RobotsControllerTests(RobotsWebApplicationFactory factory)
    {
      _factory = factory;
    }

    [Theory]
    [InlineData("v1/robots")]
    [InlineData("v1/robots/jobs")]
    public async Task Get_Robots_Returns_Success(string url)
    {
      // Arrange
      var client = _factory.CreateClient();

      // Act
      var response = await client.GetAsync(url);

      // Assert
      response.EnsureSuccessStatusCode(); // Status Code 200-299
    }

    [Fact]
    public async Task Post_Gets_Nearest_Robot()
    {
      // Arrange
      var payload = new PayLoad(1, 2, 3);
      var payLoadJson = JsonConvert.SerializeObject(payload);
      var content = new StringContent(payLoadJson, Encoding.UTF8, MediaTypeNames.Application.Json);

      // Act
      HttpResponseMessage response = await _factory.CreateClient()
                                                   .PostAsync("v1/robots/closest", content);

      // Assert
      response.EnsureSuccessStatusCode();
    }
  }
}
