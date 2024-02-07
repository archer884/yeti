namespace Yeti.Db.Model;

public class Manuscript
{
    public long Id { get; set; }
    public long WriterId { get; set; }
    public string Title { get; set; } = null!;
    public string? Summary { get; set; }
    public bool SoftDelete { get; set; }

    public ICollection<Fragment> Fragments { get; set; } = [];
    public Writer Writer { get; set; } = null!;
}
