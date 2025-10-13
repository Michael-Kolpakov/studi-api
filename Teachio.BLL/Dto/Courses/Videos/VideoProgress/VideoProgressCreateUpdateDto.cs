namespace Teachio.BLL.Dto.Courses.Videos.VideoProgress;

public abstract class VideoProgressCreateUpdateDto
{
    public Guid VideoId { get; set; }

    public int PositionSeconds { get; set; }
}
