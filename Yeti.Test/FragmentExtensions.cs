using Yeti.Db.Model;

namespace Yeti.Core.Service;

public class FragmentExtensions
{
    public class WithEmptyCollection
    {
        [Fact]
        public void NextSortValueIsZero()
        {
            List<TestSortable> sortable = [];
            Assert.Equal(0.0, sortable.NextSortBy());
        }
    }

    public class WithNonEmptyCollection
    {
        [Fact]
        public void NextSortValueIsNonZero()
        {
            var sortable = Enumerable.Range(0, 5).Select(x => new TestSortable((double)x)).ToList();
            var next = sortable.NextSortBy();
            var difference = Math.Abs(5.0 - next);
            Assert.True(difference < double.Epsilon, $"{difference} > {double.Epsilon} ({next})");
        }
    }

    public record TestSortable(double SortBy) : ISortable;
}
