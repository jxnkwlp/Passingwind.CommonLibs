using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// A <see cref="ResultContext{TOptions}"/> when authentication has failed.
/// </summary>
public class ApiKeyAuthenticationFailedContext : ResultContext<ApiKeyOptions>
{
    /// <summary>
    /// Initializes a new instance of <see cref="ApiKeyAuthenticationFailedContext"/>.
    /// </summary>
    /// <inheritdoc />
    public ApiKeyAuthenticationFailedContext(
        HttpContext context,
        AuthenticationScheme scheme,
        ApiKeyOptions options)
        : base(context, scheme, options)
    {
    }

    /// <summary>
    /// Gets or sets the exception associated with the authentication failure.
    /// </summary>
    public Exception Exception { get; set; } = default!;
}
