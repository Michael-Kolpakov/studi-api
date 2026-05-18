using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Users.Account.Request.Upload;
using Teachio.BLL.DTOs.Users.Account.Response;

namespace Teachio.BLL.CQRS.Users.Account.UploadAvatar;

public record UploadAvatarCommand(AvatarUploadRequestDto AvatarUploadRequestDto)
    : IRequest<Result<AvatarUploadResponseDto>>;
