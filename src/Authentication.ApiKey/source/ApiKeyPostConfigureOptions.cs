using System;
using Microsoft.Extensions.Options;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// 
/// </summary>
public class ApiKeyPostConfigureOptions : IPostConfigureOptions<ApiKeyOptions>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    public void PostConfigure(string? name, ApiKeyOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.HeaderName) && string.IsNullOrWhiteSpace(options.QueryStringName) && string.IsNullOrWhiteSpace(options.AuthenticationSchemeName))
        {
            throw new InvalidOperationException($"API key authentication must be set {nameof(ApiKeyOptions.HeaderName)} or {nameof(ApiKeyOptions.QueryStringName)} or {nameof(ApiKeyOptions.AuthenticationSchemeName)}");
        }
    }
}