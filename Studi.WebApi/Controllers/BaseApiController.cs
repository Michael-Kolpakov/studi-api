using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Studi.BLL.CQRS.ResultValidations;
using Studi.BLL.DTOs.Shared;

namespace Studi.WebApi.Controllers;

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

    protected ActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
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
}
