using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// Extension methods to configure ApiKey authentication.
/// </summary>
public static class ApiKeyExtensions
{
    /// <summary>
    /// Enables ApiKey authentication using the default scheme
    /// </summary>
    public static AuthenticationBuilder AddApiKey<TApiKeyProvider>(this AuthenticationBuilder builder) where TApiKeyProvider : class, IApiKeyProvider
    {
        return AddApiKey<TApiKeyProvider>(builder: builder, authenticationScheme: ApiKeyDefaults.AuthenticationScheme, displayName: null, configureOptions: _ => { });
    }

    /// <summary>
    /// Enables ApiKey authentication
    /// </summary>
    public static AuthenticationBuilder AddApiKey<TApiKeyProvider>(this AuthenticationBuilder builder, string authenticationScheme) where TApiKeyProvider : class, IApiKeyProvider
    {
        return AddApiKey<TApiKeyProvider>(builder: builder, authenticationScheme: authenticationScheme, displayName: null, configureOptions: _ => { });
    }

    /// <summary>
    /// Enables ApiKey authentication
    /// </summary>
    public static AuthenticationBuilder AddApiKey<TApiKeyProvider>(this AuthenticationBuilder builder, Action<ApiKeyOptions> configureOptions) where TApiKeyProvider : class, IApiKeyProvider
    {
        return AddApiKey<TApiKeyProvider>(builder: builder, authenticationScheme: ApiKeyDefaults.AuthenticationScheme, displayName: null, configureOptions: configureOptions);
    }

    /// <summary>
    /// Enables ApiKey authentication
    /// </summary>
    public static AuthenticationBuilder AddApiKey<TApiKeyProvider>(this AuthenticationBuilder builder, string authenticationScheme, Action<ApiKeyOptions> configureOptions) where TApiKeyProvider : class, IApiKeyProvider
    {
        return AddApiKey<TApiKeyProvider>(builder: builder, authenticationScheme: authenticationScheme, displayName: null, configureOptions: configureOptions);
    }

    /// <summary>
    /// Enables ApiKey authentication
    /// </summary>
    public static AuthenticationBuilder AddApiKey<TApiKeyProvider>(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<ApiKeyOptions> configureOptions) where TApiKeyProvider : class, IApiKeyProvider
    {
        builder.Services.TryAddTransient<IApiKeyProvider, TApiKeyProvider>();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<ApiKeyOptions>, ApiKeyPostConfigureOptions>());

        builder.AddScheme<ApiKeyOptions, ApiKeyHandler>(authenticationScheme, displayName, configureOptions);

        builder.Services.AddTransient<ApiKeyHandler>();

        return builder;
    }
}