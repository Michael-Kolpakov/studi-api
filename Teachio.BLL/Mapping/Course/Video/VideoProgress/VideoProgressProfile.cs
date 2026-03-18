using AutoMapper;
using Teachio.BLL.Dto.Courses.Videos.VideoProgress.Request.Update;
using Teachio.BLL.Dto.Courses.Videos.VideoProgress.Response;
using VideoProgressEntity = Teachio.DAL.Entities.Courses.Videos.VideoProgress.VideoProgress;

namespace Teachio.BLL.Mapping.Course.Video.VideoProgress;

/// <summary>
/// Represents the <see cref="VideoProgressProfile"/> type.
/// </summary>
public class VideoProgressProfile : Profile
{
    public VideoProgressProfile()
    {
        CreateMap<VideoProgressUpdateRequestDto, VideoProgressEntity>();

        CreateMap<VideoProgressEntity, VideoProgressResponseDto>();
    }
}
