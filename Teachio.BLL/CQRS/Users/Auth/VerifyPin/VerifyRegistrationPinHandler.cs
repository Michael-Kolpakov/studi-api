using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Users.Auth.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Entities.Users.Users;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.CQRS.Users.Auth.VerifyPin;

public class VerifyRegistrationPinHandler : IRequestHandler<VerifyRegistrationPinCommand, Result<AuthTokenPairDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IPinCodeService _pinCodeService;
    private readonly IAuthTokenIssuer _authTokenIssuer;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<AuthSharedResource> _stringLocalizerAuth;

    public VerifyRegistrationPinHandler(
        UserManager<AppUser> userManager,
        IRepositoryWrapper repositoryWrapper,
        IPinCodeService pinCodeService,
        IAuthTokenIssuer authTokenIssuer,
        ILoggerService logger,
        IStringLocalizer<AuthSharedResource> stringLocalizerAuth)
    {
        _userManager = userManager;
        _repositoryWrapper = repositoryWrapper;
        _pinCodeService = pinCodeService;
        _authTokenIssuer = authTokenIssuer;
        _logger = logger;
        _stringLocalizerAuth = stringLocalizerAuth;
    }

    public async Task<Result<AuthTokenPairDto>> Handle(VerifyRegistrationPinCommand request, CancellationToken cancellationToken)
    {
        var verificationId = request.VerifyRegistrationPinRequestDto.VerificationId;
        _logger.LogInformation($"Entered '{GetType().Name}' to verify registration PIN with Id: {verificationId}");

        if (verificationId == Guid.Empty)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.RegistrationPinInvalid)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var pendingRegistration = await _repositoryWrapper.PendingRegistrationsRepository.GetSingleOrDefaultAsync(
            pr => pr.Id == verificationId,
            cancellationToken: cancellationToken);

        if (pendingRegistration is null)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.RegistrationPinNotFound)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (pendingRegistration.PinExpiresAtUtc <= DateTime.UtcNow)
        {
            _repositoryWrapper.PendingRegistrationsRepository.Delete(pendingRegistration);
            await _repositoryWrapper.SaveChangesAsync(cancellationToken);

            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.RegistrationPinExpired)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (!_pinCodeService.IsValidPin(request.VerifyRegistrationPinRequestDto.PinCode)
            || !_pinCodeService.VerifyPin(request.VerifyRegistrationPinRequestDto.PinCode, pendingRegistration.PinHash))
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.RegistrationPinInvalid)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var existingUser = await _userManager.FindByEmailAsync(pendingRegistration.Email);

        if (existingUser is not null)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.UserWithEmailAlreadyExists),
                pendingRegistration.Email
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var normalizedEmail = pendingRegistration.NormalizedEmail;
        var normalizedUserName = _userManager.NormalizeName(pendingRegistration.Email) ?? normalizedEmail;
        var user = new AppUser()
        {
            Name = pendingRegistration.Name,
            Surname = pendingRegistration.Surname,
            Email = pendingRegistration.Email,
            UserName = pendingRegistration.Email,
            NormalizedEmail = normalizedEmail,
            NormalizedUserName = normalizedUserName,
            EmailConfirmed = true,
            PasswordHash = pendingRegistration.PasswordHash
        };

        var createResult = await _userManager.CreateAsync(user);

        if (!createResult.Succeeded)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.UserRegistrationFailed)
            ].Value;

            var errorDetails = string.Join(
                "; ",
                createResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

            _logger.LogError(request, $"{errorMessage} {errorDetails}");

            return Result.Fail(errorMessage);
        }

        _repositoryWrapper.PendingRegistrationsRepository.Delete(pendingRegistration);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        return await _authTokenIssuer.IssueTokenPairAsync(user, cancellationToken);
    }
}
