using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Users.Account.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Entities.Users.Users;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.CQRS.Users.Account.Delete;

public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand, Result<AppUserResponseDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<AuthSharedResource> _stringLocalizerAuth;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public DeleteAccountHandler(
        UserManager<AppUser> userManager,
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<AuthSharedResource> stringLocalizerAuth,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind)
    {
        _userManager = userManager;
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerAuth = stringLocalizerAuth;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
    }

    public async Task<Result<AppUserResponseDto>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to delete a user with Id: {userId}");

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

        var passwordIsValid = await _userManager.CheckPasswordAsync(user, request.AccountDeleteRequestDto.Password);

        if (!passwordIsValid)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.CurrentPasswordInvalid)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        await CleanupUserDataAsync(userId, cancellationToken);

        var userFolderDeletionResult = await DeleteUserFolderFromCDNAsync(user.Email!, request, cancellationToken);
        if (userFolderDeletionResult.IsFailed)
        {
            var errorMessage = userFolderDeletionResult.Errors[0].Message;

            return Result.Fail(errorMessage);
        }

        var responseDto = _mapper.Map<AppUserResponseDto>(user);

        var deleteResult = await _userManager.DeleteAsync(user);

        if (!deleteResult.Succeeded)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.UserDeleteFailed)
            ].Value;

            var errorDetails = string.Join(
                "; ",
                deleteResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

            _logger.LogError(request, $"{errorMessage} {errorDetails}");

            return Result.Fail(errorMessage);
        }

        return Result.Ok(responseDto);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "CDN is a constant abbreviation and it's ok to use it in the method name for better readability and understanding of the method's purpose.")]
    private async Task<Result> DeleteUserFolderFromCDNAsync(
        string ownerUserEmail,
        DeleteAccountCommand request,
        CancellationToken cancellationToken)
    {
        var userFolderDeletionResult = await _googleDriveStorageService.DeleteFolderByPathAsync(
            StoragePathHelper.BuildUserFolderSegments(ownerUserEmail),
            cancellationToken);

        if (userFolderDeletionResult.IsFailed)
        {
            var errorMessage = userFolderDeletionResult.Errors[0].Message;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }

    private async Task CleanupUserDataAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userEnrollments = (await _repositoryWrapper.UserCoursesRepository.GetAllAsync(
                userCourse => userCourse.AppUserId == userId,
                include: query => query.Include(userCourse => userCourse.Course),
                cancellationToken))
            .ToList();

        var userVideoProgresses = (await _repositoryWrapper.VideoProgressRepository.GetAllAsync(
                videoProgress => videoProgress.AppUserId == userId,
                cancellationToken: cancellationToken))
            .ToList();

        var affectedCourses = userEnrollments
            .Where(userCourse => userCourse.Course.OwnerUserId != userId)
            .Select(userCourse => userCourse.Course)
            .DistinctBy(course => course.Id)
            .ToList();

        if (userEnrollments.Count > 0)
        {
            _repositoryWrapper.UserCoursesRepository.DeleteRange(userEnrollments);
        }

        if (userVideoProgresses.Count > 0)
        {
            _repositoryWrapper.VideoProgressRepository.DeleteRange(userVideoProgresses);
        }

        foreach (var course in affectedCourses)
        {
            course.WatchingUsersCount = Math.Max(0, course.WatchingUsersCount - 1);
            _repositoryWrapper.CoursesRepository.Update(course);
        }
    }
}
