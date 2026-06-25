using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Yeti.Core.Service;

namespace Yeti.Web.Pages;

public class TagModel(TagSearchService tagSearchService) : PageModel
{
    const int PageSize = 25;

    public string Value { get; set; } = "";
    public ManuscriptListPage List { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string value, int p = 0)
    {
        Value = value;
        List = await Build(value, p);
        return Page();
    }

    public async Task<IActionResult> OnGetPageAsync(string value, int p) =>
        Partial("_ManuscriptListPage", await Build(value, p));

    async Task<ManuscriptListPage> Build(string value, int p)
    {
        var items = await tagSearchService.ByTag(value, p);
        var more = items.Count == PageSize ? $"/Tag/{Uri.EscapeDataString(value)}?handler=Page&p={p + 1}" : null;
        return new ManuscriptListPage(items, more);
    }
}
