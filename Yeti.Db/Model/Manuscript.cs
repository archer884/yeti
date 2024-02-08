namespace Yeti.Db.Model;

public class Manuscript : Tracked
{
    public long Id { get; set; }
    public long WriterId { get; set; }
    public string Title { get; set; } = null!;
    public string? Blurb { get; set; }
    public bool SoftDelete { get; set; }

    public List<Fragment> Fragments { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];
    
    public Writer Writer { get; set; } = null!;
}
