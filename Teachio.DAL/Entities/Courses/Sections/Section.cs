using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.DAL.Entities.Courses.Sections;

/// <summary>
/// Represents the <see cref="Section"/> type.
/// </summary>
public class Section
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string SectionName { get; set; } = null!;

    public int OrderIndex { get; set; }

    public int VideosCount { get; set; }

    public Guid CourseId { get; set; }

    public Course? Course { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<Video> Videos { get; set; } = new List<Video>();
}
