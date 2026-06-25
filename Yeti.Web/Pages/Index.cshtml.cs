using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Yeti.Core.Model;
using Yeti.Core.Provider;
using Yeti.Core.Service;

namespace Yeti.Web.Pages;

public class IndexModel(RecentService recentService) : PageModel
{
    public List<ManuscriptSummary> New { get; set; } = [];
    public List<ManuscriptSummary> Updated { get; set; } = [];

    public async Task OnGetAsync()
    {
        New = await recentService.ByNew(0);
        Updated = await recentService.ByUpdated(0);
    }
}
