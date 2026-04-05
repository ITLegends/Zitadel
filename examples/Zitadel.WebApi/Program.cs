using Duende.AspNetCore.Authentication.OAuth2Introspection;

using Zitadel.Abstractions.Credentials;
using Zitadel.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services
    .AddAuthorization()
    .AddAuthentication()
    .AddZitadelIntrospection(
        "ZITADEL_JWT",
        o =>
        {
            o.Authority = "https://zitadel-instance-abcdef.zitadel.cloud";
            o.ClientId = "123456789012345678@yourproject";
            o.JwtProfile = new ZitadelApplication.JwtProfile
            {
                KeyId = "123456789012345678",
                // Optionally provide a SignJwt delegate instead to use an external vault
                Key = "-----BEGIN RSA PRIVATE KEY-----\nMY SUPER SECRET KEY\n-----END RSA PRIVATE KEY-----\n",
                AppId = "123456789012345678"
            };
        })
    .AddOAuth2Introspection(
        "ZITADEL_BASIC",
        o =>
        {
            o.Authority = "https://zitadel-instance-abcdef.zitadel.cloud";
            o.ClientId = "123456789012345678@yourproject";
            o.ClientSecret = "myclientsecret";
        })
    .AddZitadelFake("ZITADEL_FAKE",
        o =>
        {
            o.FakeZitadelId = "1337";
        });

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
