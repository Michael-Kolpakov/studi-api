using AutoMapper;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.BLL.Utils.MappingResolvers;

/// <summary>
/// Represents the <see cref="OwnerUserIdResolver"/> type.
/// </summary>
public class OwnerUserIdResolver : IValueResolver<object, Course, Guid>
{
    /// <summary>
    /// Maps the input data to the target representation.
    /// </summary>
    /// <param name="source">The source object to map from.</param>
    /// <param name="destination">The destination object to map to.</param>
    /// <param name="destMember">The destination member value.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The result produced by this operation.</returns>
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
