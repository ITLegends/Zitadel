using Microsoft.AspNetCore.Identity;

using Zitadel.Abstractions;
using Zitadel.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services
    .AddAuthorization()
    .AddAuthentication(ZitadelDefaults.AuthenticationScheme)
    .AddZitadel(
        o =>
        {
            o.Authority = "https://zitadel-instance-abcdef.zitadel.cloud";
            o.ClientId = "123456789012345678@yourproject";
            o.SignInScheme = IdentityConstants.ExternalScheme;
            o.SaveTokens = true;
        })
    .AddExternalCookie()
    .Configure(
        o =>
        {
            o.Cookie.HttpOnly = true;
            o.Cookie.IsEssential = true;
            o.Cookie.SameSite = SameSiteMode.None;
            o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

await app.RunAsync();
