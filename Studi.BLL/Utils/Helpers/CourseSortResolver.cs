using System.Linq.Expressions;
using Studi.BLL.CQRS.Courses.Courses.GetPaginated;
using Studi.BLL.Utils.MappingResolvers;
using CourseEntity = Studi.DAL.Entities.Courses.Courses.Course;

namespace Studi.BLL.Utils.Helpers;

public static class CourseSortResolver
{
    public static SortDirection ResolveSortDirection(CoursesSortBy sortBy, SortDirection sortDirection)
    {
        if (sortBy == CoursesSortBy.None)
        {
            return SortDirection.None;
        }

        return sortDirection == SortDirection.None
            ? SortDirection.Descending
            : sortDirection;
    }

    public static (Expression<Func<CourseEntity, object>>? Ascending, Expression<Func<CourseEntity, object>>? Descending) ResolveSortSelectors(
        CoursesSortBy sortBy,
        SortDirection sortDirection)
    {
        if (sortBy == CoursesSortBy.None || sortDirection == SortDirection.None)
        {
            return (null, null);
        }

        Expression<Func<CourseEntity, object>>? sortKeySelector = sortBy switch
        {
            CoursesSortBy.None => null,
            CoursesSortBy.TotalDurationHours => GetTotalDurationHoursSortKeySelector(),
            CoursesSortBy.WatchingUsersCount => course => course.WatchingUsersCount,
            _ => null
        };

        if (sortKeySelector is null)
        {
            return (null, null);
        }

        return sortDirection == SortDirection.Ascending
            ? (sortKeySelector, null)
            : (null, sortKeySelector);
    }

    public static (
        Expression<Func<CourseEntity, object>>? PrimaryAscending,
        Expression<Func<CourseEntity, object>>? PrimaryDescending,
        Expression<Func<CourseEntity, object>>? SecondaryAscending,
        Expression<Func<CourseEntity, object>>? SecondaryDescending)
        BuildSortSelectors(CoursesSortBy sortBy, SortDirection sortDirection)
    {
        var (ascendingSortKeySelector, descendingSortKeySelector) = ResolveSortSelectors(sortBy, sortDirection);

        var updatedAtSelector = GetUpdatedAtSortKeySelector();

        Expression<Func<CourseEntity, object>>? primaryAscending = ascendingSortKeySelector;
        Expression<Func<CourseEntity, object>>? primaryDescending = descendingSortKeySelector;
        Expression<Func<CourseEntity, object>>? secondaryAscending = null;
        Expression<Func<CourseEntity, object>>? secondaryDescending = null;

        if (sortBy == CoursesSortBy.None)
        {
            primaryAscending = null;
            primaryDescending = updatedAtSelector;
        }
        else
        {
            if (primaryAscending is not null || primaryDescending is not null)
            {
                secondaryDescending = updatedAtSelector;
            }
        }

        return (primaryAscending, primaryDescending, secondaryAscending, secondaryDescending);
    }

    private static Expression<Func<CourseEntity, object>> GetTotalDurationHoursSortKeySelector()
    {
        var durationExpression = TotalDurationHoursResolver.TotalDurationHoursExpression;

        return Expression.Lambda<Func<CourseEntity, object>>(
            Expression.Convert(durationExpression.Body, typeof(object)),
            durationExpression.Parameters);
    }

    private static Expression<Func<CourseEntity, object>> GetUpdatedAtSortKeySelector()
    {
        return course => course.UpdatedAt;
    }
}
