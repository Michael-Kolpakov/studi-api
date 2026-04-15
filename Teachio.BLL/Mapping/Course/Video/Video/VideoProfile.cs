using AutoMapper;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Create;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Update;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using Teachio.BLL.Utils.MappingResolvers;
using VideoEntity = Teachio.DAL.Entities.Courses.Videos.Videos.Video;

namespace Teachio.BLL.Mapping.Course.Video.Video;

public class VideoProfile : Profile
{
    public VideoProfile()
    {
        CreateMap<VideoCreateRequestDto, VideoEntity>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom<TrimmedTitleResolver>());

        CreateMap<VideoUpdateRequestDto, VideoEntity>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom<TrimmedTitleResolver>());

        CreateMap<VideoEntity, VideoResponseDto>()
            .ForMember(
                dest => dest.VideoRelativePath,
                opt => opt.MapFrom<RelativePathResolver>());

        CreateMap<VideoEntity, VideoPreviewResponseDto>();

        CreateMap<VideoEntity, VideoShortResponseDto>();
    }
}
