using Studi.DAL.Entities.Courses.Sections;
using Studi.DAL.Entities.Courses.Videos.Videos;

namespace Studi.BLL.Utils.Helpers;

public static class OrderIndexHelper
{
    public static void NormalizeOrderIndexes(List<Section> sections)
    {
        NormalizeOrderIndexes(
            sections,
            s => s.OrderIndex,
            (s, i) => s.OrderIndex = i);
    }

    public static void NormalizeOrderIndexes(List<Video> videos)
    {
        NormalizeOrderIndexes(
            videos,
            v => v.OrderIndex,
            (v, i) => v.OrderIndex = i);
    }

    private static void NormalizeOrderIndexes<T>(List<T> items, Func<T, int> getIndex, Action<T, int> setIndex)
    {
        if (items is null)
        {
            return;
        }

        items.Sort((a, b) => getIndex(a).CompareTo(getIndex(b)));

        for (var i = 0; i < items.Count; i++)
        {
            setIndex(items[i], i);
        }
    }
}
