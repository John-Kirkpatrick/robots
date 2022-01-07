using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Robots.Data.Contexts;
using Robots.Domain.Entities;

namespace Robots.Core.Queries
{
  public interface IJobQueries
  {
    Task<Job[]> GetJobsAsync(CancellationToken token);
  }

  public class JobQueries : IJobQueries
  {
    private readonly RobotContext _context;

    public JobQueries(RobotContext context)
    {
      _context = context;
    }

    public async Task<Job[]> GetJobsAsync(CancellationToken token)
    {
      return await _context.Jobs.OrderByDescending(x => x.Created)
                           .ToArrayAsync(token);
    }
  }
}
