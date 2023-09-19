using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// A context for <see cref="ApiKeyEvents.OnTokenValidated"/>.
/// </summary>
public class ApiKeyTokenValidatedContext : ResultContext<ApiKeyOptions>
{
    /// <summary>
    /// Initializes a new instance of <see cref="ApiKeyTokenValidatedContext"/>.
    /// </summary>
    /// <inheritdoc />
    public ApiKeyTokenValidatedContext(
        HttpContext context,
        AuthenticationScheme scheme,
        ApiKeyOptions options)
        : base(context, scheme, options) { }

    /// <summary>
    /// Gets or sets the validated apikey token.
    /// </summary>
    public string Token { get; set; } = default!;
}
