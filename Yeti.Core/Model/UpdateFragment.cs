namespace Yeti.Core.Model;

public record UpdateFragment(long WriterId, long FragmentId, string? Heading, string Content, double? SortBy);
