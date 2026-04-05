using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Zitadel.Abstractions;


namespace Zitadel.AspNet.AuthN.Pages;

[Authorize]
public class Authenticated : PageModel
{
    public async Task OnPostAsync()
    {
        // Options: signs you out of ZITADEL entirely, without this you may not be reprompted for your password.
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme); 
        await HttpContext.SignOutAsync(
            ZitadelDefaults.AuthenticationScheme,
            new AuthenticationProperties { RedirectUri = "/loggedout" }
        );
    }
}
