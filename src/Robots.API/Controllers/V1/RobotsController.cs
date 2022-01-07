using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Robots.Core.Queries;
using Robots.Core.Services;
using Robots.Domain.Entities;
using Robots.Domain.Models;

namespace Robots.API.Controllers.V1
{
  [Route("v1/[controller]")]
  [ApiController]
  [Produces(MediaTypeNames.Application.Json)]
  public class RobotsController : Controller
  {
    private readonly IRobotQueries _queries;
    private readonly IRobotService _robotService;
    private readonly IJobQueries _jobQueries;

    public RobotsController(IRobotService robotService, IRobotQueries queries, IJobQueries jobQueries)
    {
      _robotService = robotService;
      _queries = queries;
      _jobQueries = jobQueries;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<RobotContract[]>> GetRobots()
    {
      var results = await _queries.GetRobotsAsync(default);
      return Ok(results);
    }

    [HttpPost("closest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
    public async Task<ActionResult<NearestRobotResult>> GetNearestRobot([FromBody] PayLoad payload)
    {
      var robot = await _robotService.GetNearestRobotAsync(payload, default);
      return Ok(robot);
    }

    [HttpGet("jobs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<Job[]>> GetJobs()
    {
      var results = await _jobQueries.GetJobsAsync(default);
      return Ok(results);
    }
  }
}