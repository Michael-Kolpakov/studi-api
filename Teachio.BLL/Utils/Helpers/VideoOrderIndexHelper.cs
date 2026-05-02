using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.BLL.Utils.Helpers;

public static class VideoOrderIndexHelper
{
    public static void NormalizeOrderIndexes(List<Video> videos)
    {
        for (var index = 0; index < videos.Count; index++)
        {
            videos[index].OrderIndex = index;
        }
    }
}
