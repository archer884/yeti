using Yeti.Db.Model;

namespace Yeti.Core.Service;

public static class FragmentExtensions
{
    public static double NextSortBy<T>(this IList<T> sortable) where T : ISortable
    {
        if (sortable.Count == 0)
        {
            return 0.0;
        }

        return sortable.Select(x => x.SortBy).Max() + 1.0;
    }
}
