using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Robots.API.Application.Filters
{
  public class HttpGlobalExceptionFilter : IExceptionFilter
  {
    private readonly ILogger<HttpGlobalExceptionFilter> _logger;

    public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
    {
      _logger = logger;
    }

    /// <summary>
    ///   Called after an action has thrown an <see cref="T:System.Exception" />.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ExceptionContext" />.</param>
    public void OnException(ExceptionContext context)
    {
      _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);

      if (context.Exception.GetType() == typeof(ValidationException))
      {
        var problemDetails = new ValidationProblemDetails {
          Instance = context.HttpContext.Request.Path,
          Status = StatusCodes.Status400BadRequest,
          Detail = "Please refer to the errors property for additional details."
        };

        var errors = new List<string> { context.Exception.Message };

        var validationErrors = ((ValidationException) context.Exception.GetBaseException()).Errors.ToList();

        errors.AddRange(validationErrors.Select(error => error.ErrorMessage));
        problemDetails.Errors.Add("DomainValidations", errors.ToArray());
        context.Result = new BadRequestObjectResult(problemDetails);
        context.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
      }
      else if (context.Exception.GetType() == typeof(KeyNotFoundException))
      {
        var problemDetails = new ValidationProblemDetails {
          Instance = context.HttpContext.Request.Path,
          Status = StatusCodes.Status404NotFound,
          Detail = "Please refer to the errors property for additional details."
        };

        problemDetails.Errors.Add("DomainValidations", new[] { context.Exception.GetBaseException().Message });
        context.Result = new BadRequestObjectResult(problemDetails);
        context.HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
      }
      else
      {
        var json = new JsonErrorResponse {
          Messages = new[]
          {
            "An error occur.Try it again."
          },
          DeveloperMessage = context.Exception
        };

        context.Result = new InternalServerErrorObjectResult(json);
        context.HttpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
      }

      context.ExceptionHandled = true;
    }

    private class JsonErrorResponse
    {
      public object? DeveloperMessage { get; set; }

      public string[]? Messages { get; set; }
    }
  }

  public class InternalServerErrorObjectResult : ObjectResult
  {
    public InternalServerErrorObjectResult(object error) : base(error)
    {
      StatusCode = StatusCodes.Status500InternalServerError;
    }
  }
}
