using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// A <see cref="ResultContext{TOptions}"/> when access to a resource is forbidden.
/// </summary>
public class ApiKeyForbiddenContext : ResultContext<ApiKeyOptions>
{
    /// <summary>
    /// Initializes a new instance of <see cref="ApiKeyForbiddenContext"/>.
    /// </summary>
    /// <inheritdoc />
    public ApiKeyForbiddenContext(HttpContext context, AuthenticationScheme scheme, ApiKeyOptions options) : base(context, scheme, options)
    {
    }
}
