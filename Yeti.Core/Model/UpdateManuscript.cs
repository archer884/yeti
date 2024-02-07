namespace Yeti.Core.Model;

public record UpdateManuscript(long WriterId, long ManuscriptId, string Title, string? Blurb);
