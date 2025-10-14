using AutoMapper;
using Teachio.BLL.Dto.Courses.Videos.Videos;
using Teachio.BLL.Dto.Courses.Videos.Videos.Create;
using Teachio.BLL.Dto.Courses.Videos.Videos.Update;
using VideoEntity = Teachio.DAL.Entities.Courses.Videos.Videos.Video;

namespace Teachio.BLL.Mapping.Course.Video.Video;

public class VideoProfile : Profile
{
    public VideoProfile()
    {
        CreateMap<VideoCreateDto, VideoEntity>();
        CreateMap<VideoUpdateDto, VideoEntity>();
        CreateMap<VideoEntity, VideoResponseDto>();
    }
}
