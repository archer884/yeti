using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Yeti.Core.Model;
using Yeti.Core.Service;

namespace Yeti.Web.Pages;

public class IndexModel(RecentService recentService) : PageModel
{
    const int PageSize = 10;

    public ManuscriptListPage NewPage { get; set; } = null!;
    public ManuscriptListPage UpdatedPage { get; set; } = null!;

    public async Task OnGetAsync()
    {
        NewPage = await Build(recentService.ByNew, "NewPage", 0);
        UpdatedPage = await Build(recentService.ByUpdated, "UpdatedPage", 0);
    }

    public async Task<IActionResult> OnGetNewPageAsync(int p) =>
        Partial("_ManuscriptListPage", await Build(recentService.ByNew, "NewPage", p));

    public async Task<IActionResult> OnGetUpdatedPageAsync(int p) =>
        Partial("_ManuscriptListPage", await Build(recentService.ByUpdated, "UpdatedPage", p));

    async Task<ManuscriptListPage> Build(Func<int, Task<List<ManuscriptSummary>>> fetch, string handler, int p)
    {
        var items = await fetch(p);
        var more = items.Count == PageSize ? $"/?handler={handler}&p={p + 1}" : null;
        return new ManuscriptListPage(items, more);
    }
}
