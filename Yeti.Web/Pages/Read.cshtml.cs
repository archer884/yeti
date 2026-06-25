using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Yeti.Core.Model;
using Yeti.Core.Provider;

namespace Yeti.Web.Pages;

public class ReadModel(FragmentDetailProvider fragmentDetailProvider) : PageModel
{
    public FragmentDetail? Fragment { get; set; }

    public async Task<IActionResult> OnGetAsync(long id)
    {
        Fragment = await fragmentDetailProvider.ById(id);
        return Fragment is null ? NotFound() : Page();
    }
}
