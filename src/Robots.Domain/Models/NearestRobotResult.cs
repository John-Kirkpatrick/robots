using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using FluentValidation;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Robots.Domain.Models
{
  [DataContract]
  public class NearestRobotResult : IValidatable
  {
    [Required]
    public int RobotId { get; set; }

    [Required]
    public double DistanceToGoal { get; set; }

    [Required]
    public int BatteryLevel { get; set; }

    public ValidationResult GetValidationResult()
    {
      return new NearestRobotResultValidator().Validate(this);
    }
  }

  public class NearestRobotResultValidator : AbstractValidator<NearestRobotResult>
  {
    public NearestRobotResultValidator()
    {
      RuleFor(model => model.RobotId)
        .NotNull()
        .NotEmpty()
        .WithMessage($"{nameof(NearestRobotResult.RobotId)} is required");

      RuleFor(model => model.BatteryLevel)
        .LessThanOrEqualTo(100)
        .WithMessage($"{nameof(NearestRobotResult.BatteryLevel)} can not exceed 100");
    }
  }
}
