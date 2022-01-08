using FluentValidation.Results;

namespace Robots.Domain
{
  public interface IValidatable
  {
    ValidationResult GetValidationResult();
  }
}