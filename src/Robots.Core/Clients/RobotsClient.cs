using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Robots.Domain.Models;

namespace Robots.Core.Clients
{
  public interface IRobotsClient
  {
    /// <summary>
    /// Retrieves robots
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<RobotContract[]> GetRobotsAsync(CancellationToken token);
  }

  public class RobotsClient : IRobotsClient
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<RobotsClient> _logger;

    public RobotsClient(HttpClient httpClient, ILogger<RobotsClient> logger)
    {
      _logger = logger;
      _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<RobotContract[]> GetRobotsAsync(CancellationToken token)
    {
      _logger.LogDebug("Retrieving robots");
      const string url = "robots";
      HttpResponseMessage result = await _httpClient.GetAsync(url, token);

      result.EnsureSuccessStatusCode();

      var response = await result.Content.ReadAsStringAsync(token);
      var records = JsonConvert.DeserializeObject<RobotContract[]>(response);
      return records!;
    }
  }
}
