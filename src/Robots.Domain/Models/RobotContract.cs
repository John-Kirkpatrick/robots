using FluentValidation;
using FluentValidation.Results;

namespace Robots.Domain.Models
{
  public class RobotContract : IValidatable
  {
    public int RobotId { get; set; }

    public int BatteryLevel { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public Coordinate Location => new Coordinate(X, Y);

    public ValidationResult GetValidationResult()
    {
      return new RobotModelValidator().Validate(this);
    }
  }

  public class RobotModelValidator : AbstractValidator<RobotContract>
  {
    public RobotModelValidator()
    {
      RuleFor(model => model.RobotId)
        .NotNull()
        .NotEmpty()
        .WithMessage($"{nameof(RobotContract.RobotId)} is required.");

      RuleFor(model => model.BatteryLevel)
        .NotNull()
        .NotEmpty()
        .WithMessage($"{nameof(RobotContract.BatteryLevel)} is required.");

      RuleFor(model => model.X)
        .NotNull()
        .NotEmpty()
        .WithMessage($"{nameof(RobotContract.X)} is required.");

      RuleFor(model => model.Y)
        .NotNull()
        .NotEmpty()
        .WithMessage($"{nameof(RobotContract.Y)} is required.");
    }
  }
}
