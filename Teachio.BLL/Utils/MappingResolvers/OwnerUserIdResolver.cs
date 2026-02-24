using AutoMapper;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.BLL.Utils.MappingResolvers;

public class OwnerUserIdResolver : IValueResolver<object, CourseEntity, Guid>
{
    public Guid Resolve(object source, CourseEntity destination, Guid destMember, ResolutionContext context)
    {
        if (context.Items.TryGetValue("OwnerUserId", out var ownerUserId) && ownerUserId is Guid userId)
        {
            return userId;
        }

        throw new ArgumentException(
            "OwnerUserId must be provided in mapping context via context.Items[\"OwnerUserId\"]",
            nameof(context));
    }
}
