using Microsoft.AspNetCore.Mvc;
using Olava.Application.Common;
using Olava.Application.Common.Enums;

namespace Olava.Api.Controllers;

/// <summary>
/// Shared behaviour for all controllers.
/// [ApiController] gives us: automatic model validation (400 responses),
/// automatic [FromBody] inference, and ProblemDetails error shapes.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected string? ClientIp => HttpContext.Connection.RemoteIpAddress?.ToString();

    /// <summary>Result&lt;T&gt; → 200 with the value (or a custom success response).</summary>
    protected IActionResult ToActionResult<T>(Result<T> result, Func<T, IActionResult>? onSuccess = null) =>
        result.IsSuccess
            ? onSuccess?.Invoke(result.Value) ?? Ok(result.Value)
            : ToProblem(result.Error!);

    /// <summary>Result → 204 No Content on success.</summary>
    protected IActionResult ToActionResult(Result result) =>
        result.IsSuccess ? NoContent() : ToProblem(result.Error!);

    private ObjectResult ToProblem(Error error)
    {
        var status = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = ProblemDetailsFactory.CreateProblemDetails(
            HttpContext, statusCode: status, title: error.Code, detail: error.Description);
        problem.Extensions["code"] = error.Code;

        return new ObjectResult(problem) { StatusCode = status };
    }
}