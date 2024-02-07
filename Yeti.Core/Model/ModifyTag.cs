namespace Yeti.Core.Model;

public record ModifyTag(long WriterId, long ManuscriptId, string Value)
{
    public string Normalized => Value.ToLowerInvariant();
}
