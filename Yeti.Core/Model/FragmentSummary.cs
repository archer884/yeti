namespace Yeti.Core.Model;

public class FragmentSummary
{
    public long Id { get; init; }
    public long WriterId { get; init; }
    public long ManuscriptId { get; init; }
    public string? Heading { get; init; }
    public int Length { get; init; }    
    public double SortBy { get; init; }
}
