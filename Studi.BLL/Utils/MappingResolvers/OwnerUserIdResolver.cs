using AutoMapper;
using Studi.DAL.Entities.Courses.Courses;

namespace Studi.BLL.Utils.MappingResolvers;

public class OwnerUserIdResolver : IValueResolver<object, Course, Guid>
{
    public Guid Resolve(object source, Course destination, Guid destMember, ResolutionContext context)
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
