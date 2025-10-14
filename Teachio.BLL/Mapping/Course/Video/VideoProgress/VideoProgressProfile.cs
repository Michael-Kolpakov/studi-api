using AutoMapper;
using Teachio.BLL.Dto.Courses.Videos.VideoProgress;
using Teachio.BLL.Dto.Courses.Videos.VideoProgress.Create;
using Teachio.BLL.Dto.Courses.Videos.VideoProgress.Update;
using VideoProgressEntity = Teachio.DAL.Entities.Courses.Videos.VideoProgress.VideoProgress;

namespace Teachio.BLL.Mapping.Course.Video.VideoProgress;

public class VideoProgressProfile : Profile
{
    public VideoProgressProfile()
    {
        CreateMap<VideoProgressCreateDto, VideoProgressEntity>();
        CreateMap<VideoProgressUpdateDto, VideoProgressEntity>();
        CreateMap<VideoProgressEntity, VideoProgressResponseDto>();
    }
}
