using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Robots.Core.Behaviors
{
  public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  {
    private readonly IValidator<TRequest>[]? _validators;
    private readonly bool _hasValidator = true;

    public ValidatorBehavior()
    {
      _hasValidator = false;
    }

    public ValidatorBehavior(IValidator<TRequest>[] validators)
    {
      _validators = validators;
    }

    public ValidatorBehavior(IValidator<TRequest> validator)
    {
      _validators = new[]
      {
        validator
      };
    }

    public async Task<TResponse> Handle(
      TRequest request,
      CancellationToken cancellationToken,
      RequestHandlerDelegate<TResponse> next
    )
    {
      if (_hasValidator && _validators != null)
      {
        var failures = _validators!.Select(v => v.Validate(request))
                                   .SelectMany(result => result.Errors)
                                   .Where(error => error != null)
                                   .ToList();

        if (failures.Any())
        {
          throw new ValidationException($"Command Validation Errors for type {typeof(TRequest).Name}", failures);
        }
      }

      TResponse response = await next();
      return response;
    }
  }
}