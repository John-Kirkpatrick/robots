using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Robots.Data.Contexts;
using Robots.Domain.Entities;

namespace Robots.Core.Commands
{
  public record StoreJobCommand : IRequest<Guid>
  {
    public StoreJobCommand(Guid jobId, int payloadId, int robotId, double distanceToGoal, int batteryLevel)
    {
      JobId = jobId;
      PayloadId = payloadId;
      RobotId = robotId;
      DistanceToGoal = distanceToGoal;
      BatteryLevel = batteryLevel;
    }

    public Guid JobId { get; init; }

    public int PayloadId { get; init; }

    public int RobotId { get; init; }

    public double DistanceToGoal { get; init; }

    public int BatteryLevel { get; init; }
  }

  public class StoreJobHandler : IRequestHandler<StoreJobCommand, Guid>
  {
    private readonly RobotContext _context;

    public StoreJobHandler(RobotContext context)
    {
      _context = context;
    }

    public async Task<Guid> Handle(StoreJobCommand command, CancellationToken token)
    {
      // idempotence
      var item = await _context.Jobs.AsNoTracking()
                               .FirstOrDefaultAsync(x => x.PayloadId == command.PayloadId || x.Id == command.JobId, token);

      if (item == null)
      {
        item = new Job {
          Id = command.JobId,
          PayloadId = command.PayloadId,
          RobotId = command.RobotId,
          DistanceToGoal = command.DistanceToGoal,
          BatteryLevel = command.BatteryLevel,
          Created = DateTime.UtcNow,
        };

        _context.Jobs.Add(item);
        await _context.SaveChangesAsync(token);
      }

      return item.Id;
    }
  }

  public class StoreJobValidator : AbstractValidator<StoreJobCommand>
  {
    public StoreJobValidator()
    {
      RuleFor(e => e.JobId)
        .NotNull()
        .NotEmpty()
        .WithMessage($"{nameof(StoreJobCommand.JobId)} is required.");

      RuleFor(e => e.PayloadId)
        .NotNull()
        .NotEmpty()
        .WithMessage($"{nameof(StoreJobCommand.PayloadId)} is required.");

      RuleFor(e => e.RobotId)
        .NotNull()
        .NotEmpty()
        .WithMessage($"{nameof(StoreJobCommand.RobotId)} is required.");
    }
  }
}
