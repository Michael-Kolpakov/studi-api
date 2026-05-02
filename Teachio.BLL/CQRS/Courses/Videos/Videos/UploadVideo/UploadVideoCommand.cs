using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Request.Upload;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Teachio.BLL.CQRS.Courses.Videos.Videos.UploadVideo;

public record UploadVideoCommand(VideoUploadRequestDto VideoUploadRequestDto)
    : IRequest<Result<VideoUploadResponseDto>>;
