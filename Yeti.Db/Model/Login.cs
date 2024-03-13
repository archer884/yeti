using System.ComponentModel.DataAnnotations;

namespace Yeti.Db.Model;

public class Login
{
    public long Id { get; init; }
    public long WriterId { get; init; }

    /// <summary>
    /// The serialized form of the password hash and salt.
    /// </summary>
    [Length(0, 128)]
    public string Serialized { get; init; } = string.Empty;

    public Writer Writer { get; init; } = null!;
}
