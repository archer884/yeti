namespace Yeti.Core.Model;

public record Snapshot(
    long Id,
    long WriterId,
    long ManuscriptId,
    string? AuthorName,
    string Title,
    string? Heading,
    string Excerpt);
