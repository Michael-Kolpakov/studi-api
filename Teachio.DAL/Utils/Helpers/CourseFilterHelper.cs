using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Utils.Constants;

namespace Teachio.DAL.Utils.Helpers;

public static class CourseFilterHelper
{
    public static Expression<Func<Course, bool>> BuildCoursesPredicate(
        Guid userId,
        bool isInProgressMode,
        string? normalizedTitleFilter,
        bool excludeCoursesWithoutVideos = false)
    {
        if (!string.IsNullOrWhiteSpace(normalizedTitleFilter))
        {
            var likePattern = $"%{normalizedTitleFilter}%";

            if (isInProgressMode)
            {
                if (excludeCoursesWithoutVideos)
                {
                    return course => course.WatchingUsers.Any(user => user.Id == userId)
                        && EF.Functions.Like(
                            EF.Functions.Collate(course.Title, DatabaseConstants.CaseInsensitiveCollation),
                            likePattern)
                        && course.Sections.Any(s => s.VideosCount > 0);
                }

                return course => course.WatchingUsers.Any(user => user.Id == userId)
                    && EF.Functions.Like(
                        EF.Functions.Collate(course.Title, DatabaseConstants.CaseInsensitiveCollation),
                        likePattern);
            }

            if (excludeCoursesWithoutVideos)
            {
                return course => !course.WatchingUsers.Any(user => user.Id == userId)
                    && EF.Functions.Like(
                        EF.Functions.Collate(course.Title, DatabaseConstants.CaseInsensitiveCollation),
                        likePattern)
                    && course.Sections.Any(s => s.VideosCount > 0);
            }

            return course => !course.WatchingUsers.Any(user => user.Id == userId)
                && EF.Functions.Like(
                    EF.Functions.Collate(course.Title, DatabaseConstants.CaseInsensitiveCollation),
                    likePattern);
        }

        if (isInProgressMode)
        {
            if (excludeCoursesWithoutVideos)
            {
                return course => course.WatchingUsers.Any(user => user.Id == userId)
                    && course.Sections.Any(s => s.VideosCount > 0);
            }

            return course => course.WatchingUsers.Any(user => user.Id == userId);
        }

        if (excludeCoursesWithoutVideos)
        {
            return course => !course.WatchingUsers.Any(user => user.Id == userId)
                && course.Sections.Any(s => s.VideosCount > 0);
        }

        return course => !course.WatchingUsers.Any(user => user.Id == userId);
    }

    public static Expression<Func<Course, bool>> BuildAnonymousCoursesPredicate(
        string? normalizedTitleFilter,
        bool excludeCoursesWithoutVideos = false)
    {
        if (!string.IsNullOrWhiteSpace(normalizedTitleFilter))
        {
            var likePattern = $"%{normalizedTitleFilter}%";

            if (excludeCoursesWithoutVideos)
            {
                return course => EF.Functions.Like(
                        EF.Functions.Collate(course.Title, DatabaseConstants.CaseInsensitiveCollation),
                        likePattern)
                    && course.Sections.Any(s => s.VideosCount > 0);
            }

            return course => EF.Functions.Like(
                EF.Functions.Collate(course.Title, DatabaseConstants.CaseInsensitiveCollation),
                likePattern);
        }

        if (excludeCoursesWithoutVideos)
        {
            return course => course.Sections.Any(s => s.VideosCount > 0);
        }

        return _ => true;
    }
}
