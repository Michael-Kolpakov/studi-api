using AutoMapper;
using Studi.BLL.Utils.Helpers;
using Studi.DAL.Entities.Courses.Courses;

namespace Studi.BLL.Utils.MappingResolvers;

public class OwnerFullNameResolver : IValueResolver<Course, object, string>
{
    public string Resolve(Course source, object destination, string destMember, ResolutionContext context)
    {
        return UserFullNameHelper.BuildFullName(
            source.OwnerUser?.Name,
            source.OwnerUser?.Surname);
    }
}
