namespace Yeti.Db.Model;

public class Tag
{
    public long Id { get; set; }
    public string Value { get; set; } = null!;

    public List<Manuscript> Manuscripts { get; set; } = [];
}
