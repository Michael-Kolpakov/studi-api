using AutoMapper;
using Studi.BLL.DTOs.Courses.Videos.VideoProgress.Request.Update;
using Studi.BLL.DTOs.Courses.Videos.VideoProgress.Response;
using VideoProgressEntity = Studi.DAL.Entities.Courses.Videos.VideoProgress.VideoProgress;

namespace Studi.BLL.Mappings.Course.Video.VideoProgress;

public class VideoProgressProfile : Profile
{
    public VideoProgressProfile()
    {
        CreateMap<VideoProgressUpdateRequestDto, VideoProgressEntity>();

        CreateMap<VideoProgressEntity, VideoProgressResponseDto>();
    }
}
