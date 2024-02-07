namespace Yeti.Db.Model;

public class Fragment
{
    public long Id { get; set; }
    public long WriterId { get; set; }
    public long ManuscriptId { get; set; }
    public string? Heading { get; set; }
    public string Content { get; set; } = null!;

    /// <summary>
    /// Sort value of a fragment determines the order in which it appears in the manuscript.
    /// </summary>
    public double SortBy { get; set; }

    public Manuscript Manuscript { get; set; } = null!;
    public Writer Writer { get; set; } = null!;
}
