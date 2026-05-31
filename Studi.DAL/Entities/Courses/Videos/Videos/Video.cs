using Studi.DAL.Entities.Courses.Sections;
using Studi.DAL.Entities.Courses.Videos.VideoFiles;

namespace Studi.DAL.Entities.Courses.Videos.Videos;

public class Video
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public Guid SectionId { get; set; }

    public Section? Section { get; set; }

    public int OrderIndex { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public VideoFile? VideoFile { get; set; }

    public List<VideoProgress.VideoProgress> VideoProgresses { get; set; } = new List<VideoProgress.VideoProgress>();
}
