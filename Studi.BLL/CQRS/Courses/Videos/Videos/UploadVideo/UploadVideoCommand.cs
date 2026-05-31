using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Videos.Videos.Request.Upload;
using Studi.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Studi.BLL.CQRS.Courses.Videos.Videos.UploadVideo;

public record UploadVideoCommand(VideoUploadRequestDto VideoUploadRequestDto)
    : IRequest<Result<VideoUploadResponseDto>>;
