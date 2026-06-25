using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Yeti.Core.Model;
using Yeti.Core.Provider;
using Yeti.Core.Service;

namespace Yeti.Web.Pages;

public class IndexModel(
    RecentService recentService,
    ManuscriptSummaryProvider manuscriptSummaryProvider) : PageModel
{
    const int PageSize = 10;

    public ManuscriptListPage NewPage { get; set; } = null!;
    public ManuscriptListPage UpdatedPage { get; set; } = null!;
    public ManuscriptListPage? YourPage { get; set; }

    long? WriterId => long.TryParse(User.FindFirst("id")?.Value, out var id) ? id : null;

    public async Task OnGetAsync()
    {
        NewPage = await Build(recentService.ByNew, "NewPage", 0);
        UpdatedPage = await Build(recentService.ByUpdated, "UpdatedPage", 0);

        if (WriterId is long id)
        {
            var mine = await manuscriptSummaryProvider.ByWriterId(id);
            YourPage = new ManuscriptListPage(mine, null);
        }
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
