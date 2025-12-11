using AutoMapper;
using Teachio.BLL.Dto.Courses.Sections.Request.Create;
using Teachio.BLL.Dto.Courses.Sections.Request.Update;
using Teachio.BLL.Dto.Courses.Sections.Response;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.Mapping.Course.Section;

public class SectionProfile : Profile
{
    public SectionProfile()
    {
        CreateMap<SectionCreateRequestDto, SectionEntity>();
        CreateMap<SectionUpdateRequestDto, SectionEntity>();
        CreateMap<SectionEntity, SectionResponseDto>();
        CreateMap<SectionEntity, SectionPreviewResponseDto>();
        CreateMap<SectionEntity, SectionShortResponseDto>();
    }
}
