using AutoMapper;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Request.Create;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Request.Update;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;
using Teachio.BLL.Utils.MappingResolvers;
using VideoEntity = Teachio.DAL.Entities.Courses.Videos.Videos.Video;

namespace Teachio.BLL.Mappings.Course.Video.Video;

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
                dest => dest.DurationSeconds,
                opt => opt.MapFrom(src => src.VideoFile!.DurationSeconds));

        CreateMap<VideoEntity, VideoPreviewResponseDto>()
            .ForMember(
                dest => dest.DurationSeconds,
                opt => opt.MapFrom(src => src.VideoFile!.DurationSeconds));

        CreateMap<VideoEntity, VideoShortResponseDto>()
            .ForMember(
                dest => dest.DurationSeconds,
                opt => opt.MapFrom(src => src.VideoFile!.DurationSeconds));

        CreateMap<VideoEntity, VideoEditShortResponseDto>()
            .ForMember(
                dest => dest.DurationSeconds,
                opt => opt.MapFrom(src => src.VideoFile!.DurationSeconds));
    }
}
