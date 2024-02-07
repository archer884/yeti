namespace Yeti.Core.Model;

public record CreateFragment(long WriterId, long ManuscriptId, string? Heading, string Content);
