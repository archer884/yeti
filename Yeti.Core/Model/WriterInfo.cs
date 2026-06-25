using Yeti.Db.Model;

namespace Yeti.Core.Model;

public record WriterInfo(string Username, string? AuthorName)
{
    public static WriterInfo FromWriter(Writer writer) => new(writer.Username, writer.AuthorName);
}
