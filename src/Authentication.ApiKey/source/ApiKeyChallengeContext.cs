using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// A <see cref="PropertiesContext{TOptions}"/> when access to a resource authenticated using ApiKey is challenged.
/// </summary>
public class ApiKeyChallengeContext : PropertiesContext<ApiKeyOptions>
{
    /// <summary>
    /// Initializes a new instance of <see cref="ApiKeyChallengeContext"/>.
    /// </summary>
    /// <inheritdoc />
    public ApiKeyChallengeContext(HttpContext context, AuthenticationScheme scheme, ApiKeyOptions options, AuthenticationProperties? properties) : base(context, scheme, options, properties)
    {
    }

    /// <summary>
    /// Any failures encountered during the authentication process.
    /// </summary>
    public Exception? AuthenticateFailure { get; set; }

    /// <summary>
    /// Gets or sets the "error" value returned to the caller as part
    /// of the WWW-Authenticate header. 
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// If true, will skip any default logic for this challenge.
    /// </summary>
    public bool Handled { get; private set; }

    /// <summary>
    /// Skips any default logic for this challenge.
    /// </summary>
    public void HandleResponse() => Handled = true;
}