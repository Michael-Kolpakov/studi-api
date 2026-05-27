using Studi.DAL.Entities.Courses.Courses;

namespace Studi.DAL.Entities.Courses.ThumbnailFiles;

public class ThumbnailFile
{
    public Guid Id { get; set; }

    public string ThumbnailName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public string Resolution { get; set; } = null!;

    public Guid CourseId { get; set; }

    public Course? Course { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
