using System.Runtime.InteropServices;

namespace Yeti.Db.Model;

public class Fragment
{
    public long Id { get; set; }
    public long WriterId { get; set; }
    public long ManuscriptId { get; set; }
    public string? Heading { get; set; }
    public string Content { get; set; } = null!;

    public Manuscript Manuscript { get; set; } = null!;
    public Writer Writer { get; set; } = null!;
}
