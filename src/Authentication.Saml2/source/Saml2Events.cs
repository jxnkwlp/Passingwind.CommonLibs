using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ITfoxtec.Identity.Saml2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Passingwind.AspNetCore.Authentication.Saml2;

/// <summary>
/// 
/// </summary>
public class Saml2Events : RemoteAuthenticationEvents
{
    /// <summary>
    /// Invoked when a protocol message is first received.
    /// </summary>
    public Func<MessageReceivedContext, Task> OnMessageReceived { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// Invoked if exceptions are thrown during request processing. The exceptions will be re-thrown after this event unless suppressed.
    /// </summary>
    public Func<AuthenticationFailedContext, Task> OnAuthenticationFailed { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// Invoked to manipulate redirects to the identity provider for SignIn, SignOut, or Challenge.
    /// </summary>
    public Func<RedirectContext, Task> OnRedirectToIdentityProvider { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// Invoked when a wsignoutcleanup request is received at the RemoteSignOutPath endpoint.
    /// </summary>
    public Func<RemoteSignOutContext, Task> OnRemoteSignOut { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// Invoked with the security token that has been extracted from the protocol message.
    /// </summary>
    public Func<SecurityTokenReceivedContext, Task> OnSecurityTokenReceived { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// Invoked after the security token has passed validation and a ClaimsIdentity has been generated.
    /// </summary>
    public Func<SecurityTokenValidatedContext, Task> OnSecurityTokenValidated { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// Invoked if exceptions are thrown during request processing. The exceptions will be re-thrown after this event unless suppressed.
    /// </summary>
    /// <param name="context"></param>
    public virtual Task AuthenticationFailed(AuthenticationFailedContext context) => OnAuthenticationFailed(context);

    /// <summary>
    /// Invoked to manipulate redirects to the identity provider for SignIn, SignOut, or Challenge.
    /// </summary>
    /// <param name="context"></param>
    public virtual Task RedirectToIdentityProvider(RedirectContext context) => OnRedirectToIdentityProvider(context);

    /// <summary>
    /// Invoked when a protocol message is first received.
    /// </summary>
    /// <param name="context"></param>
    public virtual Task MessageReceived(MessageReceivedContext context) => OnMessageReceived(context);

    /// <summary>
    /// Invoked when a wsignoutcleanup request is received at the RemoteSignOutPath endpoint.
    /// </summary>
    /// <param name="context"></param>
    public virtual Task RemoteSignOut(RemoteSignOutContext context) => OnRemoteSignOut(context);

    /// <summary>
    /// Invoked with the security token that has been extracted from the protocol message.
    /// </summary>
    /// <param name="context"></param>
    public virtual Task SecurityTokenReceived(SecurityTokenReceivedContext context) => OnSecurityTokenReceived(context);

    /// <summary>
    /// Invoked after the security token has passed validation and a ClaimsIdentity has been generated.
    /// </summary>
    /// <param name="context"></param>
    public virtual Task SecurityTokenValidated(SecurityTokenValidatedContext context) => OnSecurityTokenValidated(context);
}

/// <summary>
/// 
/// </summary>
public class RedirectContext : PropertiesContext<Saml2Options>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="scheme"></param>
    /// <param name="options"></param>
    /// <param name="properties"></param>
    public RedirectContext(HttpContext context, AuthenticationScheme scheme, Saml2Options options, AuthenticationProperties? properties) : base(context, scheme, options, properties)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public Saml2AuthnRequest Saml2AuthnRequest { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public Saml2RedirectBinding RedirectBinding { get; set; } = default!;

    /// <summary>
    /// If true, will skip any default logic for this redirect.
    /// </summary>
    public bool Handled { get; private set; }

    /// <summary>
    /// Skips any default logic for this redirect.
    /// </summary>
    public void HandleResponse() => Handled = true;
}

/// <summary>
/// 
/// </summary>
public class RemoteSignOutContext : RemoteAuthenticationContext<Saml2Options>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="scheme"></param>
    /// <param name="options"></param>
    /// <param name="properties"></param>
    public RemoteSignOutContext(HttpContext context, AuthenticationScheme scheme, Saml2Options options, AuthenticationProperties? properties) : base(context, scheme, options, properties)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public Saml2AuthnResponse Saml2AuthnResponse { get; set; } = default!;
}

/// <summary>
/// 
/// </summary>
public class MessageReceivedContext : RemoteAuthenticationContext<Saml2Options>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="scheme"></param>
    /// <param name="options"></param>
    /// <param name="properties"></param>
    public MessageReceivedContext(HttpContext context, AuthenticationScheme scheme, Saml2Options options, AuthenticationProperties? properties) : base(context, scheme, options, properties)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public Saml2AuthnResponse Saml2AuthnResponse { get; set; } = default!;
}

/// <summary>
/// 
/// </summary>
public class SecurityTokenReceivedContext : RemoteAuthenticationContext<Saml2Options>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="scheme"></param>
    /// <param name="options"></param>
    /// <param name="properties"></param>
    public SecurityTokenReceivedContext(HttpContext context, AuthenticationScheme scheme, Saml2Options options, AuthenticationProperties? properties) : base(context, scheme, options, properties)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public Saml2AuthnResponse Saml2AuthnResponse { get; set; } = default!;
}

/// <summary>
/// 
/// </summary>
public class SecurityTokenValidatedContext : RemoteAuthenticationContext<Saml2Options>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="scheme"></param>
    /// <param name="options"></param>
    /// <param name="principal"></param>
    /// <param name="properties"></param>
    public SecurityTokenValidatedContext(HttpContext context, AuthenticationScheme scheme, Saml2Options options, ClaimsPrincipal principal, AuthenticationProperties? properties) : base(context, scheme, options, properties)
    {
        Principal = principal;
    }

    /// <summary>
    /// 
    /// </summary>
    public Saml2AuthnResponse Saml2AuthnResponse { get; set; } = default!;
}

/// <summary>
/// 
/// </summary>
public class AuthenticationFailedContext : RemoteAuthenticationContext<Saml2Options>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="scheme"></param>
    /// <param name="options"></param>
    public AuthenticationFailedContext(HttpContext context, AuthenticationScheme scheme, Saml2Options options) : base(context, scheme, options, null)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public Saml2AuthnResponse Saml2AuthnResponse { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public Exception Exception { get; set; } = default!;
}
