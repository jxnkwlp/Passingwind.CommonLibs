using System;
using Microsoft.Extensions.Options;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <inheritdoc />
public class ApiKeyPostConfigureOptions : IPostConfigureOptions<ApiKeyOptions>
{
    /// <inheritdoc />
    public void PostConfigure(string? name, ApiKeyOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.HeaderName) && string.IsNullOrWhiteSpace(options.QueryStringName) && string.IsNullOrWhiteSpace(options.HeaderAuthenticationSchemeName))
        {
            throw new InvalidOperationException($"API key authentication must be set {nameof(ApiKeyOptions.HeaderName)} or {nameof(ApiKeyOptions.QueryStringName)} or {nameof(ApiKeyOptions.HeaderAuthenticationSchemeName)}");
        }
    }
}