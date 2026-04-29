using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Upload;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.UploadVideo;

public record UploadVideoCommand(VideoUploadRequestDto VideoUploadRequestDto, Guid RequestingUserId)
    : IRequest<Result<VideoUploadResponseDto>>;
