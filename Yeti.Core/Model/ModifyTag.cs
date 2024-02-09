namespace Yeti.Core.Model;

public record ModifyTag(long ManuscriptId, string Value)
{
    public string Normalized => Value.ToLowerInvariant();
}
