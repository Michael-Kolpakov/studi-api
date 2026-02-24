using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Teachio.BLL.Dto.Shared;
using Teachio.BLL.MediatR.ResultValidations;

namespace Teachio.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;

    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if (result is NullResult<T>)
            {
                return Ok(result.Value);
            }

            return result.Value is null
                ? NotFound("Not Found")
                : Ok(result.Value);
        }

        if (result.HasError(error => error.Message == "Unauthorized"))
        {
            return Unauthorized();
        }

        return BadRequest(result.Errors.Select(x =>
            new ErrorDto()
            {
                Message = x.Message
            }
        ));
    }

    protected Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return null;
        }

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    protected Guid GetUserIdOrThrow()
    {
        var userId = GetUserId();

        return userId ?? throw new UnauthorizedAccessException("User is not authenticated");
    }
}
