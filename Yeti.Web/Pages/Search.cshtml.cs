using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Yeti.Core.Service;

namespace Yeti.Web.Pages;

public class SearchModel(SearchService searchService) : PageModel
{
    const int PageSize = 10;

    public string? Query { get; set; }
    public SearchResultsPage? ResultsPage { get; set; }

    public async Task<IActionResult> OnGetAsync(string? q, int p = 0)
    {
        Query = q;
        if (!string.IsNullOrWhiteSpace(q))
        {
            ResultsPage = await Build(q, p);
        }

        return Page();
    }

    public async Task<IActionResult> OnGetResultsAsync(string q, int p = 0) =>
        Partial("_SearchResults", await Build(q, p));

    async Task<SearchResultsPage> Build(string query, int p)
    {
        var items = await searchService.Query(query, p);
        var more = items.Count == PageSize ? MoreUrl(query, p + 1) : null;
        return new SearchResultsPage(query, p, items, more);
    }

    static string MoreUrl(string query, int p) =>
        $"/Search?handler=Results&q={Uri.EscapeDataString(query)}&p={p}";
}
