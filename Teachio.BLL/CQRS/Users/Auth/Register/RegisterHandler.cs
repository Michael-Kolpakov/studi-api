using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Users.Auth.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Entities.Users.Users;

namespace Teachio.BLL.CQRS.Users.Auth.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, Result<AuthTokenPairDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IAuthTokenIssuer _authTokenIssuer;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<AuthSharedResource> _stringLocalizerAuth;

    public RegisterHandler(
        UserManager<AppUser> userManager,
        IMapper mapper,
        IAuthTokenIssuer authTokenIssuer,
        ILoggerService logger,
        IStringLocalizer<AuthSharedResource> stringLocalizerAuth)
    {
        _userManager = userManager;
        _mapper = mapper;
        _authTokenIssuer = authTokenIssuer;
        _logger = logger;
        _stringLocalizerAuth = stringLocalizerAuth;
    }

    public async Task<Result<AuthTokenPairDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.AuthRegisterRequestDto.Email;
        _logger.LogInformation($"Entered '{GetType().Name}' to register a new user with Email: {email}");

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

        var newUser = _mapper.Map<AppUser>(request.AuthRegisterRequestDto);

        if (newUser is null)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.UserRegistrationFailed)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        newUser.Email = email;
        newUser.UserName = email;

        var createResult = await _userManager.CreateAsync(newUser, request.AuthRegisterRequestDto.Password);

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

        return await _authTokenIssuer.IssueTokenPairAsync(newUser, cancellationToken);
    }
}
