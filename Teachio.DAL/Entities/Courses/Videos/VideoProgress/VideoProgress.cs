using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.DAL.Entities.Courses.Videos.VideoProgress;

/// <summary>
/// Represents the <see cref="VideoProgress"/> type.
/// </summary>
public class VideoProgress
{
    public Guid Id { get; set; }

    public Guid VideoId { get; set; }

    public Video? Video { get; set; }

    public bool IsCompleted { get; set; }

    public int PositionSeconds { get; set; }

    public DateTime UpdatedAt { get; set; }
}
