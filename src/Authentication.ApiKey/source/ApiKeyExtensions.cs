using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// 
/// </summary>
public static class ApiKeyExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TApiKeyProvider"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddApiKey<TApiKeyProvider>(this AuthenticationBuilder builder) where TApiKeyProvider : class, IApiKeyProvider
    {
        return AddApiKey<TApiKeyProvider>(builder: builder, scheme: ApiKeyDefaults.AuthenticationScheme, displayName: null, configureOptions: null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TApiKeyProvider"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddApiKey<TApiKeyProvider>(this AuthenticationBuilder builder, Action<ApiKeyOptions>? configureOptions = null) where TApiKeyProvider : class, IApiKeyProvider
    {
        return AddApiKey<TApiKeyProvider>(builder: builder, scheme: ApiKeyDefaults.AuthenticationScheme, displayName: null, configureOptions: configureOptions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TApiKeyProvider"></typeparam>
    /// <param name="builder"></param>
    /// <param name="scheme"></param>
    /// <param name="displayName"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddApiKey<TApiKeyProvider>(this AuthenticationBuilder builder, string scheme, string? displayName = null, Action<ApiKeyOptions>? configureOptions = null) where TApiKeyProvider : class, IApiKeyProvider
    {
        builder.Services.TryAddTransient<IApiKeyProvider, TApiKeyProvider>();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<ApiKeyOptions>, ApiKeyPostConfigureOptions>());

        builder.AddScheme<ApiKeyOptions, ApiKeyHandler>(scheme, displayName, configureOptions);

        builder.Services.AddTransient<ApiKeyHandler>();

        return builder;
    }
}