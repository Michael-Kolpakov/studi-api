using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.DAL.Entities.Courses.Sections;

public class Section
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public int OrderIndex { get; set; }

    public Guid CourseId { get; set; }

    public Course? Course { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<Video> Videos { get; set; } = new List<Video>();
}
