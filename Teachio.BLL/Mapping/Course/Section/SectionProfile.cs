using AutoMapper;
using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Dto.Courses.Sections.Create;
using Teachio.BLL.Dto.Courses.Sections.Update;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.Mapping.Course.Section;

public class SectionProfile : Profile
{
    public SectionProfile()
    {
        CreateMap<SectionCreateDto, SectionEntity>();
        CreateMap<SectionUpdateDto, SectionEntity>();
        CreateMap<SectionEntity, SectionResponseDto>();
    }
}
