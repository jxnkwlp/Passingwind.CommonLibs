using System;
using System.Threading.Tasks;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// Specifies events which the <see cref="ApiKeyHandler"/> invokes to enable developer control over the authentication process.
/// </summary>
public class ApiKeyEvents
{
    /// <summary>
    /// 
    /// </summary>
    public Func<ApiKeyMessageReceivedContext, Task> OnMessageReceived { get; set; } = context => Task.CompletedTask;
    /// <summary>
    /// 
    /// </summary>
    public Func<ApiKeyTokenValidatedContext, Task> OnTokenValidated { get; set; } = context => Task.CompletedTask;
    /// <summary>
    /// 
    /// </summary>
    public Func<ApiKeyAuthenticationFailedContext, Task> OnAuthenticationFailed { get; set; } = context => Task.CompletedTask;
    /// <summary>
    /// 
    /// </summary>
    public Func<ApiKeyChallengeContext, Task> OnChallenge { get; set; } = context => Task.CompletedTask;
    /// <summary>
    /// 
    /// </summary>
    public Func<ApiKeyForbiddenContext, Task> OnForbidden { get; set; } = context => Task.CompletedTask;


    /// <summary>
    /// Invoked when a protocol message is first received.
    /// </summary>
    public virtual Task MessageReceivedAsync(ApiKeyMessageReceivedContext context) => OnMessageReceived(context);
    /// <summary>
    /// Invoked after the security token has passed validation and a ClaimsIdentity has been generated.
    /// </summary>
    public virtual Task TokenValidatedAsync(ApiKeyTokenValidatedContext context) => OnTokenValidated(context);
    /// <summary>
    /// Invoked if exceptions are thrown during request processing. The exceptions will be re-thrown after this event unless suppressed.
    /// </summary>
    public virtual Task AuthenticationFailedAsync(ApiKeyAuthenticationFailedContext context) => OnAuthenticationFailed(context);
    /// <summary>
    /// Invoked before a challenge is sent back to the caller.
    /// </summary>
    public virtual Task ChallengeAsync(ApiKeyChallengeContext context) => OnChallenge(context);
    /// <summary>
    /// Invoked if Authorization fails and results in a Forbidden response
    /// </summary>
    public virtual Task ForbiddenAsync(ApiKeyForbiddenContext context) => OnForbidden(context);
}
