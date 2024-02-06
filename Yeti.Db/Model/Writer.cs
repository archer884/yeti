namespace Yeti.Db.Model;

public class Writer
{
    public long Id { get; init; }
    public string Username { get; init; } = null!;

    public virtual ICollection<Manuscript> Manuscripts { get; set; } = [];
}
