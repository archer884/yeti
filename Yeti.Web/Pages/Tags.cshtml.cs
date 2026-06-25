using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Yeti.Core.Service;

namespace Yeti.Web.Pages;

public class TagsModel(TagSearchService tagSearchService) : PageModel
{
    const int PageSize = 25;

    public List<string> Values { get; set; } = [];
    public ManuscriptListPage List { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string value, int p = 0)
    {
        Values = [.. value.Split('+')];
        List = await Build(value, p);
        return Page();
    }

    public async Task<IActionResult> OnGetPageAsync(string value, int p) =>
        Partial("_ManuscriptListPage", await Build(value, p));

    async Task<ManuscriptListPage> Build(string value, int p)
    {
        var values = value.Split('+').ToList();
        var items = await tagSearchService.ByTags(values, p);
        var more = items.Count == PageSize ? $"/Tags/{Uri.EscapeDataString(value)}?handler=Page&p={p + 1}" : null;
        return new ManuscriptListPage(items, more);
    }
}
