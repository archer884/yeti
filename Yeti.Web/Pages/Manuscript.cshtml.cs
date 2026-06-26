using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Yeti.Core.Model;
using Yeti.Core.Provider;

namespace Yeti.Web.Pages;

public class ManuscriptModel(
    ManuscriptSummaryProvider manuscriptSummaryProvider,
    FragmentSummaryProvider fragmentSummaryProvider) : PageModel
{
    public ManuscriptSummary? Manuscript { get; set; }
    public List<FragmentSummary> Fragments { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(long id)
    {
        Manuscript = await manuscriptSummaryProvider.ById(id);
        if (Manuscript is null)
        {
            return NotFound();
        }

        Fragments = await fragmentSummaryProvider.ByManuscriptId(id);
        return Page();
    }
}
