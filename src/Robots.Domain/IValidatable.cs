using FluentValidation.Results;

namespace Robots.Domain
{
  /// <summary>
  ///   Interface for models to upload to IAP
  /// </summary>
  public interface IValidatable
  {
    /// <summary>
    ///   returns the <see cref="ValidationResult" /> from model's validator
    /// </summary>
    /// <returns></returns>
    ValidationResult GetValidationResult();
  }
}