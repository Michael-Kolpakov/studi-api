using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Users.Account.Request.Update;
using Teachio.BLL.DTOs.Users.Account.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Entities.Users.Users;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.CQRS.Users.Account.Update;

public class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand, Result<AppUserResponseDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<AuthSharedResource> _stringLocalizerAuth;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public UpdateAccountHandler(
        UserManager<AppUser> userManager,
        IMapper mapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<AuthSharedResource> stringLocalizerAuth,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind)
    {
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerAuth = stringLocalizerAuth;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
    }

    public async Task<Result<AppUserResponseDto>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to update a user with Id: {userId}");

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

        var passwordCheckResult = await TryChangePasswordAsync(user, request.AccountUpdateRequestDto, request);

        if (passwordCheckResult.IsFailed)
        {
            return Result.Fail<AppUserResponseDto>(
                passwordCheckResult.Errors.Select(error => error.Message));
        }

        _mapper.Map(request.AccountUpdateRequestDto, user);

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.UserUpdateFailed)
            ].Value;

            var errorDetails = string.Join(
                "; ",
                updateResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

            _logger.LogError(request, $"{errorMessage} {errorDetails}");

            return Result.Fail(errorMessage);
        }

        var responseDto = _mapper.Map<AppUserResponseDto>(user);

        return Result.Ok(responseDto);
    }

    private async Task<Result> TryChangePasswordAsync(
        AppUser user,
        AccountUpdateRequestDto requestDto,
        UpdateAccountCommand request)
    {
        var hasCurrentPassword = !string.IsNullOrWhiteSpace(requestDto.CurrentPassword);
        var hasNewPassword = !string.IsNullOrWhiteSpace(requestDto.NewPassword);

        if (!hasCurrentPassword && !hasNewPassword)
        {
            return Result.Ok();
        }

        if (hasCurrentPassword != hasNewPassword)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.PasswordChangeRequiresBoth)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (requestDto.NewPassword!.Length > EntityConstants.MaxUserPasswordLength)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.PasswordChangeFailed)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var changeResult = await _userManager.ChangePasswordAsync(
            user,
            requestDto.CurrentPassword!,
            requestDto.NewPassword);

        if (!changeResult.Succeeded)
        {
            var isPasswordMismatch = changeResult.Errors.Any(error =>
                string.Equals(error.Code, "PasswordMismatch", StringComparison.OrdinalIgnoreCase));

            var errorMessage = isPasswordMismatch
                ? _stringLocalizerAuth[nameof(AuthSharedResource_en.CurrentPasswordInvalid)].Value
                : _stringLocalizerAuth[nameof(AuthSharedResource_en.PasswordChangeFailed)].Value;

            var errorDetails = string.Join(
                "; ",
                changeResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

            _logger.LogError(request, $"{errorMessage} {errorDetails}");

            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }
}
