using AutoMapper;
using Teachio.BLL.Dto.Courses.Videos.Videos;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Create;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Update;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using VideoEntity = Teachio.DAL.Entities.Courses.Videos.Videos.Video;

namespace Teachio.BLL.Mapping.Course.Video.Video;

public class VideoProfile : Profile
{
    public VideoProfile()
    {
        CreateMap<VideoCreateRequestDto, VideoEntity>();
        CreateMap<VideoUpdateRequestDto, VideoEntity>();
        CreateMap<VideoEntity, VideoResponseDto>();
    }
}
