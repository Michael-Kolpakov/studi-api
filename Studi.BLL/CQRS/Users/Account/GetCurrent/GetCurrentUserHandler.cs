using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Studi.BLL.DTOs.Users.Account.Response;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.DAL.Entities.Users.Users;

namespace Studi.BLL.CQRS.Users.Account.GetCurrent;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, Result<AppUserShortResponseDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public GetCurrentUserHandler(
        UserManager<AppUser> userManager,
        IMapper mapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind)
    {
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
    }

    public async Task<Result<AppUserShortResponseDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to get current user with Id: {userId}");

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindUserById),
                userId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var responseDto = _mapper.Map<AppUserShortResponseDto>(user);

        return Result.Ok(responseDto);
    }
}
