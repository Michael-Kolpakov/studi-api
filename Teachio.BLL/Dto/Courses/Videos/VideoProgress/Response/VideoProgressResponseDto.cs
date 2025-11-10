namespace Teachio.BLL.Dto.Courses.Videos.VideoProgress.Response;

public class VideoProgressResponseDto
{
    public Guid Id { get; set; }

    public Guid VideoId { get; set; }

    public int PositionSeconds { get; set; }
}
