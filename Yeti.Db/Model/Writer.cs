using System.ComponentModel.DataAnnotations;

namespace Yeti.Db.Model;

public class Writer
{
    public long Id { get; init; }
    public long LoginId { get; init; }

    [Length(2, 256)]
    public string Username { get; init; } = null!;

    [Length(2, 256)]
    public string? AuthorName { get; init; }

    public Login Login { get; init; } = null!;
    public List<Manuscript> Manuscripts { get; set; } = [];
}
