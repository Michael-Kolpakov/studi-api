using Teachio.BLL.DTOs.Courses.Videos.VideoProgress.Response;
using Teachio.DAL.Entities.Courses.Videos.VideoProgress;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.BLL.Utils.Helpers;

public static class VideoProgressHelper
{
    public static List<VideoProgress> CreateForVideoAndUsers(Guid videoId, IEnumerable<Guid> userIds)
    {
        return userIds
            .Distinct()
            .Select(userId => new VideoProgress
            {
                VideoId = videoId,
                AppUserId = userId,
                IsCompleted = false,
                PositionSeconds = 0
            })
            .ToList();
    }

    public static List<VideoProgress> CreateForVideosAndUser(IEnumerable<Guid> videoIds, Guid userId)
    {
        return videoIds
            .Distinct()
            .Select(videoId => new VideoProgress
            {
                VideoId = videoId,
                AppUserId = userId,
                IsCompleted = false,
                PositionSeconds = 0
            })
            .ToList();
    }

    public static VideoProgressResponseDto BuildResponse(Video? video, Guid userId)
    {
        ArgumentNullException.ThrowIfNull(video);

        var progress = video.VideoProgresses.FirstOrDefault(videoProgress => videoProgress.AppUserId == userId)
            ?? throw new InvalidOperationException($"Video progress for user '{userId}' and video '{video.Id}' was not found.");

        return new VideoProgressResponseDto
        {
            Id = progress.Id,
            VideoId = progress.VideoId,
            IsCompleted = progress.IsCompleted,
            PositionSeconds = progress.PositionSeconds
        };
    }
}
