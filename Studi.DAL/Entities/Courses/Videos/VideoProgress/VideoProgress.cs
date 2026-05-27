using Studi.DAL.Entities.Courses.Videos.Videos;
using Studi.DAL.Entities.Users.Users;

namespace Studi.DAL.Entities.Courses.Videos.VideoProgress;

public class VideoProgress
{
    public Guid Id { get; set; }

    public Guid VideoId { get; set; }

    public Video? Video { get; set; }

    public Guid AppUserId { get; set; }

    public AppUser? AppUser { get; set; }

    public bool IsCompleted { get; set; }

    public int PositionSeconds { get; set; }

    public DateTime UpdatedAt { get; set; }
}
