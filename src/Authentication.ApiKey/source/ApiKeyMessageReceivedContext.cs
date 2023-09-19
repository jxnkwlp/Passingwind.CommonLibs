using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// A context for <see cref="ApiKeyEvents.OnMessageReceived"/>.
/// </summary>
public class ApiKeyMessageReceivedContext : ResultContext<ApiKeyOptions>
{
    /// <summary>
    /// Initializes a new instance of <see cref="ApiKeyMessageReceivedContext"/>.
    /// </summary>
    /// <inheritdoc />
    public ApiKeyMessageReceivedContext(HttpContext context, AuthenticationScheme scheme, ApiKeyOptions options) : base(context, scheme, options)
    {
    }
}
