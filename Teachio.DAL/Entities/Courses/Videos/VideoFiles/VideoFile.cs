using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.DAL.Entities.Courses.Videos.VideoFiles;

public class VideoFile
{
    public Guid Id { get; set; }

    public string VideoName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public int DurationSeconds { get; set; }

    public Guid VideoId { get; set; }

    public Video? Video { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
