using AutoMapper;
using Studi.BLL.DTOs.Courses.Videos.Videos.Request.Create;
using Studi.BLL.DTOs.Courses.Videos.Videos.Request.Update;
using Studi.BLL.DTOs.Courses.Videos.Videos.Response;
using Studi.BLL.Utils.MappingResolvers;
using VideoEntity = Studi.DAL.Entities.Courses.Videos.Videos.Video;

namespace Studi.BLL.Mappings.Course.Video.Video;

public class VideoProfile : Profile
{
    public VideoProfile()
    {
        CreateMap<VideoCreateRequestDto, VideoEntity>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom<TrimmedTitleResolver>())
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom<TrimmedDescriptionResolver>());

        CreateMap<VideoUpdateRequestDto, VideoEntity>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom<TrimmedTitleResolver>())
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom<TrimmedDescriptionResolver>());

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
