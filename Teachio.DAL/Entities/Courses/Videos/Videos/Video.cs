using Teachio.DAL.Entities.Courses.Sections;

namespace Teachio.DAL.Entities.Courses.Videos.Videos;

public class Video
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string VideoName { get; set; } = null!;

    public string? ContentType { get; set; }

    public Guid SectionId { get; set; }

    public Section? Section { get; set; }

    public int OrderIndex { get; set; }

    public int DurationSeconds { get; set; }

    public VideoStatus Status { get; set; }

    public string? ProcessingError { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public VideoProgress.VideoProgress VideoProgress { get; set; } = null!;
}

public enum VideoStatus
{
    Uploaded,
    Processing,
    Ready,
    Failed,
    Deleted
}
