using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Yeti.Core.Service;

namespace Yeti.Web.Pages;

public class LoginModel(LoginService loginService) : PageModel
{
    const string CookieScheme = "Cookie";

    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    [TempData]
    public string? Error { get; set; }

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        var writerId = await loginService.Validate(Username, Password);
        if (writerId is null)
        {
            Error = "Invalid username or password.";
            return RedirectToPage();
        }

        var identity = new ClaimsIdentity(new[] { new Claim("id", writerId.Value.ToString()) }, CookieScheme);
        await HttpContext.SignInAsync(CookieScheme, new ClaimsPrincipal(identity));
        return RedirectToPage("/Index");
    }
}
