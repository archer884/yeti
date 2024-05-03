using Yeti.Db.Model;

namespace Yeti.Core.Model;

public class FragmentSummary : ISortable
{
    public long Id { get; init; }
    public long WriterId { get; init; }
    public long ManuscriptId { get; init; }
    public string? Heading { get; init; }
    public int Length { get; init; }
    public double SortBy { get; init; }

    public FragmentSummary() { }

    public FragmentSummary(Fragment fragment)
    {
        Id = fragment.Id;
        WriterId = fragment.WriterId;
        ManuscriptId = fragment.ManuscriptId;
        Heading = fragment.Heading;
        Length = fragment.Content.Length;
        SortBy = fragment.SortBy;
    }
}
