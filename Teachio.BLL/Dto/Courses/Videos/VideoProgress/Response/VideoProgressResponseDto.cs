namespace Teachio.BLL.Dto.Courses.Videos.VideoProgress.Response;

/// <summary>
/// Represents the <see cref="VideoProgressResponseDto"/> type.
/// </summary>
public class VideoProgressResponseDto
{
    public Guid Id { get; set; }

    public Guid VideoId { get; set; }

    public bool IsCompleted { get; set; }

    public int PositionSeconds { get; set; }
}
