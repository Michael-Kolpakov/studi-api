using AutoMapper;
using Teachio.BLL.DTOs.Courses.Videos.VideoProgress.Request.Update;
using Teachio.BLL.DTOs.Courses.Videos.VideoProgress.Response;
using VideoProgressEntity = Teachio.DAL.Entities.Courses.Videos.VideoProgress.VideoProgress;

namespace Teachio.BLL.Mappings.Course.Video.VideoProgress;

public class VideoProgressProfile : Profile
{
    public VideoProgressProfile()
    {
        CreateMap<VideoProgressUpdateRequestDto, VideoProgressEntity>();

        CreateMap<VideoProgressEntity, VideoProgressResponseDto>();
    }
}
