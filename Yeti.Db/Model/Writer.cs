namespace Yeti.Db.Model;

public class Writer
{
    public long Id { get; init; }
    public string Username { get; init; } = null!;
    public string? AuthorName { get; init; }

    public virtual ICollection<Manuscript> Manuscripts { get; set; } = [];
}
