using Yeti.Core.Model;

namespace Yeti.Web;

/// <summary>
/// A page of manuscript summaries for a list view, with an optional URL to fetch the next page.
/// </summary>
public record ManuscriptListPage(List<ManuscriptSummary> Items, string? LoadMoreUrl);

/// <summary>
/// A page of full-text search results, with an optional URL to fetch the next page.
/// </summary>
public record SearchResultsPage(string Query, int Page, List<Snapshot> Items, string? LoadMoreUrl);
