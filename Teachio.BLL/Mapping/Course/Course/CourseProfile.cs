using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Request.Create;
using Teachio.BLL.Dto.Courses.Courses.Request.Update;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Utils.MappingResolvers;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.BLL.Mapping.Course.Course;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<CourseCreateRequestDto, CourseEntity>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom<TrimmedTitleResolver>())
            .ForMember(
                dest => dest.CourseName,
                opt => opt.MapFrom<NameFromTitleResolver>());

        CreateMap<CourseUpdateRequestDto, CourseEntity>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom<TrimmedTitleResolver>())
            .ForMember(
                dest => dest.CourseName,
                opt => opt.MapFrom<NameFromTitleResolver>());

        CreateMap<CourseEntity, CourseResponseDto>()
            .ForMember(
                dest => dest.TotalDuration,
                opt => opt.MapFrom<TotalDurationResolver>());

        CreateMap<CourseEntity, CoursePreviewResponseDto>()
            .ForMember(
                dest => dest.TotalDuration,
                opt => opt.MapFrom<TotalDurationResolver>())
            .ForMember(
                dest => dest.ThumbnailRelativePath,
                opt => opt.MapFrom<ThumbnailRelativePathResolver>());

        CreateMap<CourseEntity, CoursePreviewShortResponseDto>()
            .ForMember(
                dest => dest.TotalDuration,
                opt => opt.MapFrom<TotalDurationResolver>())
            .ForMember(
                dest => dest.ThumbnailRelativePath,
                opt => opt.MapFrom<ThumbnailRelativePathResolver>());
    }
}
