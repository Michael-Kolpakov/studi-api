using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Users.Account.Request.Upload;
using Studi.BLL.DTOs.Users.Account.Response;

namespace Studi.BLL.CQRS.Users.Account.UploadAvatar;

public record UploadAvatarCommand(AvatarUploadRequestDto AvatarUploadRequestDto)
    : IRequest<Result<AvatarUploadResponseDto>>;
