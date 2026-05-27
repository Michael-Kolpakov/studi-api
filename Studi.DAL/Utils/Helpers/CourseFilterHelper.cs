using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Utils.Constants;

namespace Studi.DAL.Utils.Helpers;

public static class CourseFilterHelper
{
    public static Expression<Func<Course, bool>> BuildAvailableCoursesPredicate(
        Guid userId,
        string? normalizedTitleFilter)
    {
        if (!string.IsNullOrWhiteSpace(normalizedTitleFilter))
        {
            var likePattern = $"%{normalizedTitleFilter}%";

            return course => !course.WatchingUsers.Any(user => user.Id == userId)
                && EF.Functions.Like(
                    EF.Functions.Collate(course.Title, DatabaseConstants.CaseInsensitiveCollation),
                    likePattern)
                && course.Sections.Any(s => s.VideosCount > 0);
        }

        return course => !course.WatchingUsers.Any(user => user.Id == userId)
            && course.Sections.Any(s => s.VideosCount > 0);
    }

    public static Expression<Func<Course, bool>> BuildInProgressCoursesPredicate(
        Guid userId,
        string? normalizedTitleFilter)
    {
        if (!string.IsNullOrWhiteSpace(normalizedTitleFilter))
        {
            var likePattern = $"%{normalizedTitleFilter}%";

            return course => course.WatchingUsers.Any(user => user.Id == userId)
                && EF.Functions.Like(
                    EF.Functions.Collate(course.Title, DatabaseConstants.CaseInsensitiveCollation),
                    likePattern)
                && course.Sections.Any(s => s.VideosCount > 0);
        }

        return course => course.WatchingUsers.Any(user => user.Id == userId)
            && course.Sections.Any(s => s.VideosCount > 0);
    }

    public static Expression<Func<Course, bool>> BuildPersonalCoursesPredicate(
        Guid userId,
        string? normalizedTitleFilter)
    {
        if (!string.IsNullOrWhiteSpace(normalizedTitleFilter))
        {
            var likePattern = $"%{normalizedTitleFilter}%";

            return course => course.OwnerUserId == userId
                && EF.Functions.Like(
                    EF.Functions.Collate(course.Title, DatabaseConstants.CaseInsensitiveCollation),
                    likePattern);
        }

        return course => course.OwnerUserId == userId;
    }

    public static Expression<Func<Course, bool>> BuildAnonymousCoursesPredicate(
        string? normalizedTitleFilter)
    {
        if (!string.IsNullOrWhiteSpace(normalizedTitleFilter))
        {
            var likePattern = $"%{normalizedTitleFilter}%";

            return course => EF.Functions.Like(
                    EF.Functions.Collate(course.Title, DatabaseConstants.CaseInsensitiveCollation),
                    likePattern)
                && course.Sections.Any(s => s.VideosCount > 0);
        }

        return course => course.Sections.Any(s => s.VideosCount > 0);
    }
}
