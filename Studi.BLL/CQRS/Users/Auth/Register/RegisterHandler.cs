using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Studi.BLL.DTOs.Users.Auth.Response;
using Studi.BLL.Models.Auth;
using Studi.BLL.Models.Email;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.BLL.Utils.Helpers;
using Studi.DAL.Entities.Users.PendingRegistrations;
using Studi.DAL.Entities.Users.Users;
using Studi.DAL.Repositories.Interfaces.Base;

namespace Studi.BLL.CQRS.Users.Auth.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, Result<RegistrationPinResponseDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IPinCodeService _pinCodeService;
    private readonly IEmailService _emailService;
    private readonly EmailVerificationOptions _verificationOptions;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<AuthSharedResource> _stringLocalizerAuth;

    public RegisterHandler(
        UserManager<AppUser> userManager,
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IPinCodeService pinCodeService,
        IEmailService emailService,
        IOptions<EmailVerificationOptions> verificationOptions,
        ILoggerService logger,
        IStringLocalizer<AuthSharedResource> stringLocalizerAuth)
    {
        _userManager = userManager;
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _pinCodeService = pinCodeService;
        _emailService = emailService;
        _verificationOptions = verificationOptions?.Value ?? throw new ArgumentNullException(nameof(verificationOptions));
        _logger = logger;
        _stringLocalizerAuth = stringLocalizerAuth;
    }

    public async Task<Result<RegistrationPinResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.AuthRegisterRequestDto.Email;
        _logger.LogInformation($"Entered '{GetType().Name}' to initiate registration PIN for Email: {email}");

        await CleanupExpiredRegistrationsAsync(cancellationToken);

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.UserWithEmailAlreadyExists),
                email
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var userCandidate = _mapper.Map<AppUser>(request.AuthRegisterRequestDto);

        if (userCandidate is null)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.UserRegistrationFailed)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        userCandidate.Email = email;
        userCandidate.UserName = email;

        var validationResult = await ValidateUserAndPasswordAsync(userCandidate, request.AuthRegisterRequestDto.Password, request);

        if (validationResult.IsFailed)
        {
            return Result.Fail(validationResult.Errors[0].Message);
        }

        var normalizedEmail = _userManager.NormalizeEmail(email) ?? email.ToUpperInvariant();

        var pendingRegistration = await _repositoryWrapper.PendingRegistrationsRepository.GetSingleOrDefaultAsync(
            pr => pr.NormalizedEmail == normalizedEmail,
            cancellationToken: cancellationToken);

        var now = DateTime.UtcNow;
        var resendAvailableAtUtc = now.AddMinutes(_verificationOptions.ResendCooldownMinutes);

        if (pendingRegistration is not null)
        {
            var existingResendAvailableAtUtc = pendingRegistration.PinSentAtUtc
                .AddMinutes(_verificationOptions.ResendCooldownMinutes);

            if (existingResendAvailableAtUtc > now)
            {
                var errorMessage = _stringLocalizerAuth[
                    nameof(AuthSharedResource_en.RegistrationPinResendTooSoon)
                ].Value;

                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }
        }

        var pinCode = _pinCodeService.GeneratePin();
        var pinHash = _pinCodeService.HashPin(pinCode);
        var passwordHash = _userManager.PasswordHasher.HashPassword(userCandidate, request.AuthRegisterRequestDto.Password);

        var isNew = pendingRegistration is null;

        pendingRegistration ??= new PendingRegistration()
        {
            Id = Guid.NewGuid()
        };

        pendingRegistration.Email = email;
        pendingRegistration.NormalizedEmail = normalizedEmail;
        pendingRegistration.Name = request.AuthRegisterRequestDto.Name;
        pendingRegistration.Surname = request.AuthRegisterRequestDto.Surname;
        pendingRegistration.PasswordHash = passwordHash;
        pendingRegistration.PinHash = pinHash;
        pendingRegistration.PinSentAtUtc = now;
        pendingRegistration.PinExpiresAtUtc = now.AddMinutes(_verificationOptions.PinExpiresInMinutes);

        if (isNew)
        {
            _repositoryWrapper.PendingRegistrationsRepository.Create(pendingRegistration);
        }
        else
        {
            _repositoryWrapper.PendingRegistrationsRepository.Update(pendingRegistration);
        }

        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var fullName = UserFullNameHelper.BuildFullName(request.AuthRegisterRequestDto.Name, request.AuthRegisterRequestDto.Surname);
        var emailMessage = new RegistrationPinEmailMessageData
        {
            To = [email],
            FullName = fullName,
            PinCode = pinCode,
            Subject = _verificationOptions.Subject
        };

        var sendSucceeded = await _emailService.SendEmailAsync(emailMessage);

        if (!sendSucceeded)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.RegistrationPinSendFailed)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var response = new RegistrationPinResponseDto()
        {
            VerificationId = pendingRegistration.Id,
            ExpiresAtUtc = pendingRegistration.PinExpiresAtUtc,
            ResendAvailableAtUtc = resendAvailableAtUtc
        };

        return Result.Ok(response);
    }

    private async Task CleanupExpiredRegistrationsAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var expiredRegistrations = (await _repositoryWrapper.PendingRegistrationsRepository.GetAllAsync(
            pr => pr.PinExpiresAtUtc <= now,
            cancellationToken: cancellationToken)).ToList();

        if (expiredRegistrations.Count == 0)
        {
            return;
        }

        _repositoryWrapper.PendingRegistrationsRepository.DeleteRange(expiredRegistrations);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);
    }

    private async Task<Result> ValidateUserAndPasswordAsync(AppUser user, string password, RegisterCommand request)
    {
        var validationErrors = new List<IdentityError>();

        foreach (var validator in _userManager.UserValidators)
        {
            var validationResult = await validator.ValidateAsync(_userManager, user);

            if (!validationResult.Succeeded)
            {
                validationErrors.AddRange(validationResult.Errors);
            }
        }

        foreach (var validator in _userManager.PasswordValidators)
        {
            var validationResult = await validator.ValidateAsync(_userManager, user, password);

            if (!validationResult.Succeeded)
            {
                validationErrors.AddRange(validationResult.Errors);
            }
        }

        if (validationErrors.Count == 0)
        {
            return Result.Ok();
        }

        var errorMessage = _stringLocalizerAuth[
            nameof(AuthSharedResource_en.UserRegistrationFailed)
        ].Value;

        var errorDetails = string.Join(
            "; ",
            validationErrors.Select(error => $"{error.Code}: {error.Description}"));

        _logger.LogError(request, $"{errorMessage} {errorDetails}");

        return Result.Fail(errorMessage);
    }
}
