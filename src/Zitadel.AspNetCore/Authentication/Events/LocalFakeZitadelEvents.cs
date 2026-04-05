using Zitadel.AspNetCore.Authentication.Events.Context;
using Zitadel.AspNetCore.Authentication.Handler;

namespace Zitadel.AspNetCore.Authentication.Events;

/// <summary>
/// Events that are invoked by the Fake ZITADEL authentication handler.
/// </summary>
public class LocalFakeZitadelEvents
{
    /// <summary>
    /// Invoked after a ClaimsIdentity has been generated in the <see cref="LocalFakeZitadelHandler"/>.
    /// </summary>
    public Func<LocalFakeZitadelAuthContext, Task> OnZitadelFakeAuth { get; set; } = _ => Task.CompletedTask;
}
