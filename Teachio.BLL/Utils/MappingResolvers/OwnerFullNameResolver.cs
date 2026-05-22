using AutoMapper;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.BLL.Utils.MappingResolvers;

public class OwnerFullNameResolver : IValueResolver<Course, object, string>
{
    public string Resolve(Course source, object destination, string destMember, ResolutionContext context)
    {
        return UserFullNameHelper.BuildFullName(
            source.OwnerUser?.Name,
            source.OwnerUser?.Surname);
    }
}
