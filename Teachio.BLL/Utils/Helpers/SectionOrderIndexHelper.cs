using Teachio.DAL.Entities.Courses.Sections;

namespace Teachio.BLL.Utils.Helpers;

public static class SectionOrderIndexHelper
{
    public static void NormalizeOrderIndexes(List<Section> sections)
    {
        for (var index = 0; index < sections.Count; index++)
        {
            sections[index].OrderIndex = index;
        }
    }
}
